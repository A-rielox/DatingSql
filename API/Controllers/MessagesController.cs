using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessagesController(IUserRepository userRepository,
                                IMessageRepository messageRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // POST:  api/messages
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot send messages to yourself.");

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        // CAMBIAR a usar id en CreateMessageDto en lugar de RecipientUsername y 
        // aca ocupar solo getUserById

        if (recipient == null) return NotFound();

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        if (await _messageRepository.AddMessage(message))
        {
            // no estoy devolviendo ni las fotos ni el id, si las necesito hay q cambiar el sp
            // puedo devolver el id creado y ponerlo aca arriba
            var msgDto = _mapper.Map<MessageDto>(message); 

            return Ok(msgDto);
        }

        return BadRequest("Failed to send message.");
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //// GET:  api/messages
    //[HttpGet]
    //public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser(
    //    [FromQuery] MessageParams messageParams)
    //{
    //    messageParams.Username = User.GetUsername();

    //    var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);

    //    Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize,
    //                                    messages.TotalCount, messages.TotalPages));

    //    return Ok(messages);
    //}

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET:  api/messages/thread/{username}
    //[HttpGet("thread/{username}")]
    //public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread(string username)
    //{
    //    var currentUsername = User.GetUsername();

    //    var messages = await _uow.MessageRepository.GetMessageThread(currentUsername, username);

    //    // xel save q quite en el repository .GetMessageThread
    //    if (_uow.HasChanges()) await _uow.Complete();

    //    return Ok(messages);
    //}

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // DELETE:  api/messages/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await _messageRepository.GetMessage(id);

        if (message.SenderUsername != username && message.RecipientUsername != username)
        {
            return Unauthorized();
        }

        if (await _messageRepository.DeleteMessage(message, username)) return Ok();

        return BadRequest("Problem deleting the message.");
    }
}


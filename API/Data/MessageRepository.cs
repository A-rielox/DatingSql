using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Dapper;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace API.Data;

public class MessageRepository : IMessageRepository
{
    private readonly IMapper _mapper;
    private IDbConnection db;
    public MessageRepository(IConfiguration configuration, IMapper mapper)
    {
        this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        _mapper = mapper;
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<bool> AddMessage(Message message)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@senderId", message.SenderId);
        parameters.Add("@senderUsername", message.SenderUsername);
        parameters.Add("@recipientId", message.RecipientId);
        parameters.Add("@recipientUsername", message.RecipientUsername);
        parameters.Add("@content", message.Content);

        var succes = await db.QueryAsync<int>("sp_addMsg",
                                               parameters,
                                               commandType: CommandType.StoredProcedure);

        return succes.FirstOrDefault() == 1 ? true : false;
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<bool> DeleteMessage(Message message, string userNameDeleting)
    {
        var succes = await db.QueryAsync<int>("sp_deleteMsg",
                                    new { msgId = message.Id, userNameDeleting },
                                    commandType: CommandType.StoredProcedure);

        return succes.FirstOrDefault() > 0 ? true : false;
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<Message> GetMessage(int id)
    {
        var msg = await db.QueryAsync<Message>("sp_getMsg",
                                    new { msgId = id },
                                    commandType: CommandType.StoredProcedure);

        return msg.FirstOrDefault();
    }
    /*
    public async Task<Message> GetMessage(int id)
    {
        Message msg;
        List<Photo> photos;


        using (var lists = await db.QueryMultipleAsync("sp_getMsg",
                                    commandType: CommandType.StoredProcedure))
        {
            msg = lists.Read<Message>().FirstOrDefault();
            photos = lists.Read<Photo>().ToList();
        }

        // depende de si quiero sacar msg o msgDto
        // x ahora no ocupo foto

        return msg;

        //msg.ForEach(u =>
        //{
        //    u.Photos = photos.Where(p => p.AppUserId == u.Id)
        //                     .ToList();
        //});
    }
    */
    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<IEnumerable<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        List<Message> messages;
        List<Photo> photos;

        using (var lists = await db.QueryMultipleAsync("sp_getMsgsForUser",
                                    new { username = messageParams.Username, container  = messageParams.Container },
                                    commandType: CommandType.StoredProcedure))
        {
            messages = lists.Read<Message>().ToList();
            photos = lists.Read<Photo>().ToList();
        }

        var messagesDto = _mapper.Map<List<MessageDto>>(messages);

        messagesDto.ForEach(m =>
        {
            m.SenderPhotoUrl = photos.Where(p => p.AppUserId == m.SenderId)
                                     .FirstOrDefault().Url;

            m.RecipientPhotoUrl = photos.Where(p => p.AppUserId == m.RecipientId)
                                     .FirstOrDefault().Url; ;
        });

        return messagesDto.OrderBy(m => m.MessageSent);



        /*
        var query = _context.Messages.OrderBy(m => m.MessageSent)
                                     .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(m => m.RecipientUsername == messageParams.Username && m.RecipientDeleted == false),
            "Outbox" => query.Where(m => m.SenderUsername == messageParams.Username && m.SenderDeleted == false),
            _ => query.Where(m => m.RecipientUsername == messageParams.Username && m.RecipientDeleted == false
                                                            && m.DateRead == null) // los no leidos 
        };

        var messagesQuery = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        var pagedMessages = await PagedList<MessageDto>
                              .CreateAsync(messagesQuery, messageParams.PageNumber,
                                           messageParams.PageSize);

        return pagedMessages;
        */
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
    {
        throw new NotImplementedException();
    }
}

/*
public class MessageRepository : IMessageRepository
{
    ...
    
    /////////////////////////////////////////////////////
    /////////////////////////////////////////////////////
    //
    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages.OrderBy(m => m.MessageSent)
                                     .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(m => m.RecipientUsername == messageParams.Username && m.RecipientDeleted == false),
            "Outbox"=> query.Where(m => m.SenderUsername == messageParams.Username && m.SenderDeleted == false),
            _       => query.Where(m => m.RecipientUsername == messageParams.Username && m.RecipientDeleted == false
                                                            && m.DateRead == null)
        };

        var messagesQuery = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        var pagedMessages = await PagedList<MessageDto>
                              .CreateAsync(messagesQuery, messageParams.PageNumber,
                                           messageParams.PageSize);

        return pagedMessages;
    }


    /////////////////////////////////////////////////////
    /////////////////////////////////////////////////////
    //
    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName,
                                                                string recipientUserName)
    {
        var query = _context.Messages
            .Where(
                m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false &&
                m.SenderUsername == recipientUserName ||
                m.RecipientUsername == recipientUserName && m.SenderDeleted == false &&
                m.SenderUsername == currentUserName
            )
            .OrderBy(m => m.MessageSent)
            .AsQueryable();

        var unreadMessages = query.Where(m => m.DateRead == null &&
                                m.RecipientUsername == currentUserName).ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }

            //await _context.SaveChangesAsync();  ---- x UnitOfWork guardo en el controller (MessagesController)
        }

        return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    =====              previo ProjectTo
    {
        var messages = await _context.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(
                m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false &&
                m.SenderUsername == recipientUserName ||
                m.RecipientUsername == recipientUserName && m.SenderDeleted == false &&
                m.SenderUsername == currentUserName
            )
            .OrderByDescending(m => m.MessageSent)
            .ToListAsync();

        var unreadMessages = messages.Where(m => m.DateRead == null &&
                                m.RecipientUsername == currentUserName).ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }

            //await _context.SaveChangesAsync();  ---- x UnitOfWork guardo en el controller (MessagesController)
        }

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }
   ==========

*/
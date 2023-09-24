import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
   selector: 'app-member-messages',
   standalone: true,
   templateUrl: './member-messages.component.html',
   styleUrls: ['./member-messages.component.css'],
   imports: [CommonModule, FormsModule],
})
export class MemberMessagesComponent implements OnInit {
   @ViewChild('messageForm') messageForm?: NgForm;
   @Input() username?: string;
   @Input() messages: Message[] = [];

   messageContent = '';

   constructor(private messageService: MessageService) {}

   ngOnInit(): void {
      // this.loadMessages();
   }

   // MOVIDO A MEMBER-DETAIL.COMPONENT.TS p' q se carguen los mensages solo al activar la tab de msgs
   // loadMessages() {
   //    if (this.username) {
   //       this.messageService.getMessageThread(this.username).subscribe({
   //          next: (messages) => (this.messages = messages),
   //       });
   //    }
   // }

   sendMessage() {
      if (!this.username) return;

      this.messageService
         .sendMessage(this.username, this.messageContent)
         .subscribe({
            next: (msg) => {
               this.messages.push(msg);
               this.messageForm?.reset();
            },
         });
   }
}

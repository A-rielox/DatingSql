import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';

@Component({
   selector: 'app-member-detail',
   standalone: true,
   templateUrl: './member-detail.component.html',
   styleUrls: ['./member-detail.component.css'],
   imports: [CommonModule, TabsModule, GalleryModule, MemberMessagesComponent],
})
export class MemberDetailComponent implements OnInit {
   @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
   activeTab?: TabDirective;
   member: Member = {} as Member;
   images: GalleryItem[] = [];
   messages: Message[] = [];

   constructor(
      private memberService: MembersService,
      private route: ActivatedRoute,
      private messageService: MessageService
   ) {}

   //                                           ⚡⚡⚡
   // ngOnInit sucede antes d q se inicialice la view y en este punto no hay acceso a las tabs
   // { static: true } es para que se construya inmediatamente y se tenga acceso a ellas en este punto
   // pero tengo q quitar el *ngIf="member" de componente html para q SI se construya de inmediato
   // p'esto ocupo el route resolver, p' cargar la info antes de que empiece a construir el componente
   ngOnInit(): void {
      // this.loadMember();

      //          ⚡⚡⚡         carga el member desde el route resolver
      this.route.data.subscribe({
         next: (data) => (this.member = data['member']),
      });

      // cambia el tab de acuerdo al query param q venga
      this.route.queryParams.subscribe({
         next: (params) => {
            params['tab'] && this.selectTab(params['tab']);
         },
      });

      this.getImages();
   }

   /*                   estoy usando route resolver p' cargar el member
   loadMember() {
      const username = this.route.snapshot.paramMap.get('username');
      if (!username) return;

      this.memberService.getMember(username).subscribe({
         next: (res) => {
            this.member = res;

            this.getImages();
         },
      });
   }
    */

   loadMessages() {
      if (this.member) {
         this.messageService.getMessageThread(this.member.userName).subscribe({
            next: (messages) => (this.messages = messages),
         });
      }
   }

   getImages() {
      if (!this.member) return;

      for (const photo of this.member.photos) {
         this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
      }
   }

   onTabActivated(data: TabDirective) {
      this.activeTab = data;

      if (this.activeTab.heading === 'Messages') {
         this.loadMessages();
      }
   }

   selectTab(heading: string) {
      if (this.memberTabs) {
         this.memberTabs.tabs.find((t) => t.heading === heading)!.active = true;
      }
   }
}

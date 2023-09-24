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
   member: Member | undefined;
   images: GalleryItem[] = [];
   messages: Message[] = [];

   constructor(
      private memberService: MembersService,
      private route: ActivatedRoute,
      private messageService: MessageService
   ) {}

   ngOnInit(): void {
      this.loadMember();
   }

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

import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { MembersService } from 'src/app/_services/members.service';

@Component({
   selector: 'app-member-list',
   templateUrl: './member-list.component.html',
   styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
   // members$: Observable<Member[]> | undefined;
   members: Member[] = [];
   pagination: Pagination | undefined;
   pageNumber = 1;
   pageSize = 5;

   constructor(private membersService: MembersService) {}

   ngOnInit(): void {
      // this.members$ = this.membersService.getMembers();
      this.loadMembers();
   }

   loadMembers() {
      this.membersService.getMembers(this.pageNumber, this.pageSize).subscribe({
         next: (res) => {
            if (res.result && res.pagination) {
               this.members = res.result;
               this.pagination = res.pagination;
            }
         },
      });
   }

   pageChanged(e: any) {
      if (this.pageNumber !== e.page) {
         this.pageNumber = e.page;
         this.loadMembers();
      }

      // if (this.userParams && this.userParams?.pageNumber !== e.page) {
      //    this.userParams.pageNumber = e.page;
      //    // para que tambien cambie en el memberService q es donde esta "respaldado" p' el caso en q se cambia de pagina
      //    this.memberService.setUserParams(this.userParams);
      //    this.loadMembers();
   }
}

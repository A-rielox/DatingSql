import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
   selector: 'app-lists',
   templateUrl: './lists.component.html',
   styleUrls: ['./lists.component.css'],
})
export class ListsComponent implements OnInit {
   members: Member[] | undefined;
   predicate = 'liked';
   // NO PAGINE VIDEO 181, ESTOS NO SON FUNCIONALES, NO FUNCIONA LA PAGINACION
   pageNumber = 1;
   pageSize = 5;
   pagination: Pagination | undefined;

   constructor(private memberService: MembersService) {}

   ngOnInit(): void {
      this.loadLikes();
   }

   loadLikes() {
      this.memberService
         .getLikes(this.predicate /* , this.pageNumber, this.pageSize */)
         .subscribe({
            next: (res) => {
               this.members = res;
               // this.members = res.result;
               // this.pagination = res.pagination;
            },
         });
   }

   // NO PAGINE VIDEO 181
   pageChanged(e: any) {
      // if (this.pageNumber !== e.page) {
      //    this.pageNumber = e.page;
      //    this.loadLikes();
      // }
   }
}

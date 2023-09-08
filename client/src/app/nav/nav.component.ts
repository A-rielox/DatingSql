import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
   selector: 'app-nav',
   templateUrl: './nav.component.html',
   styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
   model: any = {};

   constructor(
      public accontService: AccountService,
      private router: Router,
      private toastr: ToastrService
   ) {}

   ngOnInit(): void {}

   login() {
      this.accontService.login(this.model).subscribe({
         next: (res) => {
            this.router.navigateByUrl('/members');
            console.log(res);
         },
         error: (err) => {
            // this.toastr.error(err.error);
            console.log(err);
         },
      });
   }

   logout() {
      this.router.navigateByUrl('/');
      this.accontService.logout();
   }
}

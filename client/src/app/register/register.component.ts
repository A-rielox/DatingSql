import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
   selector: 'app-register',
   templateUrl: './register.component.html',
   styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
   @Output() cancelRegister = new EventEmitter();
   @Input() usersFromHomeComp: User[] = [];

   model: any = {};

   constructor(
      private accountService: AccountService,
      private toastr: ToastrService
   ) {}

   ngOnInit(): void {}

   register() {
      this.accountService.register(this.model).subscribe({
         next: (res) => {
            console.log(res);
            this.cancel(); // cierro el register form
         },
         error: (err) => {
            this.toastr.error(err.error + '  💩');
         },
      });
   }

   cancel() {
      this.cancelRegister.emit(false);
   }
}

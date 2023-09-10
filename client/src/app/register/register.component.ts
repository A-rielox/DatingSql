import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import {
   AbstractControl,
   FormBuilder,
   FormGroup,
   ValidatorFn,
   Validators,
} from '@angular/forms';
import { Router } from '@angular/router';

@Component({
   selector: 'app-register',
   templateUrl: './register.component.html',
   styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
   @Output() cancelRegister = new EventEmitter();
   @Input() usersFromHomeComp: User[] = [];

   registerForm: FormGroup = new FormGroup({});
   maxDate: Date = new Date(); // inicializa con fecha y hora actual
   validationErrors: string[] | undefined;

   constructor(
      private accountService: AccountService,
      private fb: FormBuilder,
      private toastr: ToastrService,
      private router: Router
   ) {}

   ngOnInit(): void {
      this.initializeForm();

      this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
   }

   // prettier-ignore
   initializeForm() {
      this.registerForm = this.fb.group({
         gender: ['male'],
         username: ['', Validators.required],
         knownAs: ['', Validators.required],
         dateOfBirth: ['', Validators.required],
         city: ['', Validators.required],
         country: ['', Validators.required],
         password: [ '', [ Validators.required, Validators.minLength(4), Validators.maxLength(12) ] ],
         confirmPassword: [ '', [Validators.required, this.matchValues('password')] ],
      });

      // por si cambia el password despues de poner el confirmPassword y pasar la validacion
      this.registerForm.controls['password'].valueChanges.subscribe({
         next: () =>
            this.registerForm.controls[
               'confirmPassword'
            ].updateValueAndValidity(),
      });
   }

   matchValues(matchTo: string): ValidatorFn {
      return (control: AbstractControl) => {
         return control.value === control.parent?.get(matchTo)?.value
            ? null
            : { notMatching: true };
      };
   }

   register() {
      this.registerForm.value.dateOfBirth = this.getDateOnly(
         this.registerForm.value.dateOfBirth
      );

      this.accountService.register(this.registerForm.value).subscribe({
         next: (res) => {
            this.router.navigateByUrl('/members');
         },
         error: (err) => {
            // este es el q me manda el interceptor con el array validation error, el case 400 ... throw modalStateErrors.flat();
            this.validationErrors = err;
         },
      });
   }

   cancel() {
      this.cancelRegister.emit(false);
   }

   private getDateOnly(dob: string | undefined) {
      if (!dob) return;

      let theDob = new Date(dob);

      return new Date(
         theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset())
      )
         .toISOString()
         .slice(0, 10);
   }
}

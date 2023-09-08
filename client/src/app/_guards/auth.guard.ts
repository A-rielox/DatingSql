import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const authGuard: CanActivateFn = (route, state) => {
   const accountService = inject(AccountService);
   const toastr = inject(ToastrService);

   return accountService.currentUser$.pipe(
      map((res) => {
         if (res) return true;
         else {
            toastr.error('🧙‍♂️ You shall not pass!!! 💥⚡⚡💥');

            return false;
         }
      })
   );
};

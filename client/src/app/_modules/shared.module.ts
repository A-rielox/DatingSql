import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ToastrModule } from 'ngx-toastr';

import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
// import { NgxSpinnerModule } from 'ngx-spinner';

@NgModule({
   declarations: [],
   imports: [
      CommonModule,
      BsDropdownModule.forRoot(),
      TabsModule.forRoot(),
      //
      ToastrModule.forRoot({
         positionClass: 'toast-bottom-right',
      }),
      // NgxSpinnerModule.forRoot({
      //    type: 'pacman',
      // }),
   ],
   exports: [BsDropdownModule, ToastrModule, TabsModule],
})
export class SharedModule {}
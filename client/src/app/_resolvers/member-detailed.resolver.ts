import { ResolveFn } from '@angular/router';
import { Member } from '../_models/member';
import { inject } from '@angular/core';
import { MembersService } from '../_services/members.service';

export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {
   const memberService = inject(MembersService);

   // esta haciendo la llamada con username null y me genera "system.nullreferenceException"
   // x q queryParamMap es null en este caso
   // return memberService.getMember(route.queryParamMap.get('username')!);

   return memberService.getMember(route.paramMap.get('username')!);
};

/*
@Injectable({
   providedIn: 'root',
})
export class MemberDetailedResolver implements Resolve<Member> {
   constructor(private memberService: MembersService) {}

   resolve(route: ActivatedRouteSnapshot): Observable<Member> {
      const memName = route.paramMap.get('username');

      return this.memberService.getMember(memName!);
   }
}
*/

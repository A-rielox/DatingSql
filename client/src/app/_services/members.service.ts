import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
   providedIn: 'root',
})
export class MembersService {
   baseUrl = environment.apiUrl;
   members: Member[] = [];
   memberCache = new Map();

   user: User | undefined;
   userParams: UserParams | undefined;

   constructor(
      private http: HttpClient,
      private accountService: AccountService
   ) {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
         next: (res) => {
            if (res) {
               this.userParams = new UserParams(res);
               this.user = res;
            }
         },
      });
   }

   getMembers(userParams: UserParams) {
      const response = this.memberCache.get(
         Object.values(userParams).join('-')
      );

      if (response) return of(response);

      let params = getPaginationHeaders(
         userParams.pageNumber,
         userParams.pageSize
      );

      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);

      return getPaginatedResult<Member[]>(
         this.baseUrl + 'users',
         params,
         this.http
      ).pipe(
         map((res) => {
            this.memberCache.set(Object.values(userParams).join('-'), res);

            return res;
         })
      );
   }

   getMember(username: string) {
      const member = [...this.memberCache.values()]
         .reduce((t, i) => {
            return t.concat(i.result);
         }, [])
         .find((member: Member) => member.userName === username);

      if (member) return of(member);

      return this.http.get<Member>(this.baseUrl + 'users/' + username);
   }

   updateMember(member: Member) {
      return this.http.put(this.baseUrl + 'users', member).pipe(
         map(() => {
            const index = this.members.indexOf(member);

            this.members[index] = { ...this.members[index], ...member };
         })
      );
   }

   //////////////////////////////////////
   //////////     PARAMS
   //////////////////////////////////////
   getUserParams() {
      return this.userParams;
   }

   setUserParams(params: UserParams) {
      this.userParams = params;
   }

   resetUserParams() {
      if (this.user) {
         this.userParams = new UserParams(this.user);

         return this.userParams;
      }

      return;
   }

   //////////////////////////////////////
   //////////     PHOTOS
   //////////////////////////////////////
   setMainPhoto(photoId: number) {
      return this.http.put(
         this.baseUrl + 'users/set-main-photo/' + photoId,
         {}
      );
   }

   deletePhoto(photoId: number) {
      return this.http.delete(
         this.baseUrl + 'users/delete-photo/' + photoId,
         {}
      );
   }

   //////////////////////////////////////
   //////////     LIKES
   //////////////////////////////////////
   addLike(username: string) {
      return this.http.post(this.baseUrl + 'likes/' + username, {});
   }

   //                                                 NO PAGINE VIDEO 181
   getLikes(predicate: string /* , pageNumber: number, pageSize: number */) {
      return this.http.get<Member[]>(
         this.baseUrl + 'likes?predicate=' + predicate
      );

      // let params = getPaginationHeaders(pageNumber, pageSize);

      // params = params.append('predicate', predicate);

      // return getPaginatedResult<Member[]>(
      //    this.baseUrl + 'likes',
      //    params,
      //    this.http
      // );
   }

   //////////////////////////////////////
   //////////     PAGINATIONS
   //////////////////////////////////////
   // private getPaginatedResult<T>(url: string, params: HttpParams) {
   //    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

   //    return this.http.get<T>(url, { observe: 'response', params }).pipe(
   //       map((res) => {
   //          if (res.body) {
   //             paginatedResult.result = res.body;
   //          }

   //          const pagination = res.headers.get('Pagination');

   //          if (pagination) {
   //             paginatedResult.pagination = JSON.parse(pagination);
   //          }

   //          return paginatedResult;
   //       })
   //    );
   // }

   // private getPaginationHeaders(pageNumber: number, pageSize: number) {
   //    let params = new HttpParams();

   //    params = params.append('pageNumber', pageNumber);
   //    params = params.append('pageSize', pageSize);

   //    return params;
   // }
}

// getHttpOptions() {
//    const userString = localStorage.getItem('user');
//
//    if (!userString) return;
//
//    const user = JSON.parse(userString);
//
//    return {
//       headers: new HttpHeaders({
//          Authorization: 'Bearer ' + user.token,
//       }),
//    };
// }

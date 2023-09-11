import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, of } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
   providedIn: 'root',
})
export class MembersService {
   baseUrl = environment.apiUrl;
   members: Member[] = [];
   paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

   constructor(private http: HttpClient) {}

   getMembers(page?: number, itemsPerPage?: number) {
      let params = new HttpParams();

      if (page && itemsPerPage) {
         params = params.append('pageNumber', page);
         params = params.append('pageSize', itemsPerPage);
      }

      return this.http
         .get<Member[]>(this.baseUrl + 'users', { observe: 'response', params })
         .pipe(
            map((res) => {
               if (res.body) {
                  this.paginatedResult.result = res.body;
               }

               const pagination = res.headers.get('Pagination');

               if (pagination) {
                  this.paginatedResult.pagination = JSON.parse(pagination);
               }

               return this.paginatedResult;
            })
         );
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

   getMember(username: string) {
      const member = this.members.find((member) => member.userName == username);

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
   //////////     PAGINATIONS
   //////////////////////////////////////
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

import {Injectable, numberAttribute} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";

export interface InvitableUser {
  id: number
  fullName: string
  profileUrl: string
}

export interface Pagination {
  current_page: number
  page_size: number
}

@Injectable()
export class UserService {

  constructor(private readonly http: HttpClient) { }

  get_invitable_users(search_query: string, pagination: Pagination, group_id: number) {

    let param_search = search_query == '' ? 'searchquery='+'' : 'searchquery=' + search_query + '&';

    return this.http.get<InvitableUser[]>(environment.apiBaseUrl + '/user/?'
      + param_search
      + 'currentpage='
      + pagination.current_page
      + '&pagesize='
      + pagination.page_size
      + '&groupid='
      + group_id);
  }
}

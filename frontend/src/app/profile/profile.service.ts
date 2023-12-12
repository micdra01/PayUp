import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {GroupCard, GroupUpdate} from "../group/group.service";
import {firstValueFrom} from "rxjs";

export interface FullUser {
  id: number,
  email: String,
  fullName: string,
  phoneNumber: string,
  created: Date,
  profileUrl: string | null,
}

export interface EditUserDto {
  email: String,
  fullName: string,
  phoneNumber: string,
}

export interface EditUserImg {
  imageUrl: File | null
}

export interface TotalBalanceDto {
  amount: number
}

@Injectable()
export class ProfileService {
  constructor(private readonly http: HttpClient,) {
  }

  getCurrentUser() {
    return this.http.get<FullUser>(environment.apiBaseUrl + '/user/currentuser');
  }

  async getTotalBalance() {
    const call = this.http.get<TotalBalanceDto>(environment.apiBaseUrl + "/user/totalbalance")
    return await firstValueFrom<TotalBalanceDto>(call);
  }

  editCurrentUser(user: EditUserDto) {
    return this.http.put<FullUser>(environment.apiBaseUrl + '/user/profileinfo', user);
  }

  editUserImage(value: EditUserImg) {
    const formData = new FormData();
    Object.entries(value).forEach(([key, value]) =>
      formData.append(key, value)
    );
    return this.http.put<EditUserImg>(environment.apiBaseUrl+'/user/profileimage', formData, {
      reportProgress: true,
      observe: 'events'
    });
  }
}

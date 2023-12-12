import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {catchError, firstValueFrom, Observable, tap} from "rxjs";



export interface Notification {
  subject: string,
  body: string,
  footer: string,
  inviteReceived: Date,
  category: NotificationCategory
}

export interface NotificationSettingsDto {
  userId: number;
  inviteNotification: boolean;
  inviteNotificationEmail: boolean;
  expenseNotification: boolean;
  expenseNotificationEmail: boolean;
}

export enum NotificationCategory {
  GroupInvite = 1,
}

export interface GroupInviteDto{
  accepted: boolean,
  groupId: number
}

@Injectable()
export class NotificationService {

  constructor(private readonly http: HttpClient) { }

  async getNotifications(lastUpdate?: Date): Promise<Notification[]> {
    let url = environment.apiBaseUrl + "/user/notifications";

    if (lastUpdate) {
      const formattedLastUpdate = lastUpdate.toISOString();
      url += `?lastUpdate=${formattedLastUpdate}`;
    }
    // Make the HTTP request
    const call = this.http.get<Notification[]>(url);

    // Wait for the observable to complete and return the result
    return await firstValueFrom<Notification[]>(call);
  }

  acceptInvite(isAccepted: boolean, groupId: number): Observable<boolean> {
    let group_invite: GroupInviteDto = {
      accepted: isAccepted,
      groupId: groupId
    }
      return this.http.post<boolean>(environment.apiBaseUrl + '/user/accept-invite', group_invite);
    }

  async getNotificationSettings(): Promise<NotificationSettingsDto> {
    const call = this.http.get<NotificationSettingsDto>(`${environment.apiBaseUrl}/user/profileinfo/settings`);
    return await firstValueFrom(call);
  }

  updateUserNotificationSettings(settings: NotificationSettingsDto): Observable<boolean> {
    console.log(settings.expenseNotificationEmail);
    return this.http.put<boolean>(environment.apiBaseUrl + `/user/profileinfo/settings`, settings)

  }
}

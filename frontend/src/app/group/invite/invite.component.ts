import {Component, Input, OnInit} from '@angular/core';
import {GroupService, GroupInvitation} from "../group.service";
import {UserService, InvitableUser, Pagination} from "../../user/user.service";
import {ToastController} from "@ionic/angular";
import {firstValueFrom} from "rxjs";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-invite',
  templateUrl: './invite.component.html',
  styleUrls: ['./invite.component.scss'],
})
export class InviteComponent  implements OnInit {
  group_id: number = -1;
  search_query: string = '';
  displayed_users: InvitableUser[] = [];
  current_page: number = 0;
  default_page_size: number = 5;

  constructor(private readonly group_service: GroupService,
              private readonly user_service: UserService,
              private readonly toast: ToastController,
              private route: ActivatedRoute,
              private router: Router) {}

  ngOnInit() {
    this.group_id = Number(this.route.snapshot.paramMap.get('groupid'));
    this.navigate_to();
  }

  async invite(user_id: number) {
    let group_invite: GroupInvitation = {
      groupId: this.group_id,
      receiverId: user_id,
    }

    let success: boolean = await firstValueFrom(this.group_service.invite(group_invite));

    if (success) {
      await this.toast.create({
        message: 'Invite sent',
        color: 'success',
        duration: 5000
      });

      this.displayed_users = this.displayed_users.filter(value => value.id != user_id);
    }
  }

  page_previous() {
    this.current_page = this.current_page <= 0 ? 0 : this.current_page - 1;
    this.navigate_to();
  }

  page_next() {
    this.current_page++;
    this.navigate_to();
  }

  search() {
    this.current_page = 0;
    this.navigate_to();
  }

  async navigate_to() {
    let pagination: Pagination = {
      current_page: this.current_page,
      page_size: this.default_page_size
    }
    this.displayed_users = await firstValueFrom(
      this.user_service.get_invitable_users(this.search_query, pagination, this.group_id));
  }

  cancel() {
    this.router.navigate(['/groups/'+this.group_id])
  }
}

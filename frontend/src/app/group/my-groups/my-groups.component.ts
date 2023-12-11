import {Component, OnInit} from '@angular/core';
import {GroupCard, GroupService} from "../group.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-my-groups',
  templateUrl: './my-groups.component.html',
  styleUrls: ['./my-groups.component.scss'],
})
export class MyGroupsComponent  implements OnInit {
  mygroups: GroupCard[] = [];
  noGroups: boolean = false;

  constructor(
    private readonly service: GroupService,
    private router: Router
  ) {}

  async ngOnInit() {
    await this.getMyGroups();
  }

  async getMyGroups() {
    this.mygroups = await this.service.getMyGroups()
    if(this.mygroups.length === 0)
      this.noGroups = true
  }

  toCreate() {
    this.router.navigate(['groups/create'])
  }
}

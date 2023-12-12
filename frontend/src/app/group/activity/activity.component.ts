import {Component, Input, OnInit} from '@angular/core';
import {
  Balance,
  FullExpense,
  Group,
  GroupService,
  Transaction,
  UserInGroup,
} from "../group.service";
import {firstValueFrom} from "rxjs";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-activity',
  templateUrl: './activity.component.html',
  styleUrls: ['./activity.component.scss'],
})
export class ActivityComponent implements OnInit {
  id: any;
  group: Group | undefined;
  expenses: FullExpense[] = []
  balances: Balance[] = []
  transactionList: Transaction[] = []
  subpage = 'activity';
  loading: boolean = true;
  balancesLoaded: boolean = false;
  membersLoaded: boolean = false;
  members: UserInGroup[] = []
  isUserGroupOwner: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private readonly service: GroupService,
    private router: Router
  ) {
  }

  async ngOnInit() {
    await this.getId()
    this.group = await firstValueFrom(this.service.getGroup(this.id))
    await this.getAllExpenses()
    this.loading = false
  }

  async segmentChanged(ev: any) {
    if (ev.detail.value === 'balances' && !this.balancesLoaded) {
      this.loading = true
      await this.getTransactions()
      await this.getBalances()
      this.balancesLoaded = true
      this.loading = false
    }
    if (ev.detail.value === 'members' && !this.membersLoaded) {
      this.loading = true
      await this.getUsersInGroup()
      this.isUserGroupOwner = await this.service.isUserGroupOwner(this.id)
      this.membersLoaded = true
      this.loading = false
    }
  }

  async getId() {
    const map = await firstValueFrom(this.route.paramMap)
    this.id = map.get('groupId')
  }

  async getAllExpenses() {
    this.expenses = await this.service.getAllExpenses(this.id)
  }

  async getBalances() {
    this.balances = await this.service.getBalances(this.id)
  }

  async getUsersInGroup() {
    this.members = await this.service.getUserInGroup(this.id)
  }

  async getTransactions() {
    this.transactionList = await this.service.getAllTransactions(this.id);
  }
  toCreateExpense() {
    this.router.navigate(['groups/'+this.group?.id+'/create'])
  }

  toCreatePayment() {
    this.router.navigate(['groups/'+this.group?.id+'/settle'])
  }

  toInvite() {
    this.router.navigate(['groups/'+this.group?.id+'/invite'])
  }

}

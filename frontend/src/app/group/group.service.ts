import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";
import {KeyValue} from "@angular/common";

export interface Group {
  id: number,
  name: string,
  description: string,
  imageUrl: string | null,
  createdDate: Date
}

export interface GroupCard extends Group {
  amount: number
}

export interface UserInGroup {
  id: number,
  fullName: string,
  profileUrl: string
}

export interface CreateGroup {
  name: string,
  description: string,
  imageUrl: string | null,
  createdDate: Date
}

export interface GroupUpdate {
  name: string;
  description: string;
  imageUrl: File | null;
}

export interface CreateExpense {
  groupId: number,
  description: string,
  amount: number,
  createdDate: Date,
  isSettle: boolean
}

export interface CreateFullExpense {
  expense: CreateExpense,
  userIdsOnExpense: number[]
}

export interface FullExpense {
  expense: Expense
  usersOnExpense: UserOnExpense[]
  loggedInUser: number
}

export interface Expense {
  id: number
  userId: number
  groupId: number
  description: string
  amount: number
  createdDate: string
  fullName: string
  isSettle: boolean
}

export interface UserOnExpense {
  userId: number
  expenseId: number
  amount: number
  fullName: string
  imageUrl: string
}

export interface  Balance {
  userId: number,
  fullName: string,
  imageUrl: string,
  amount: number
}

export interface Transaction {
  payerId: number;
  payerName: string,
  amount: number;
  payeeId: number;
  payeeName: string;
}

export interface GroupInvitation {
  groupId: number
  receiverId: number
}

export interface CurrencyList{
  data: KeyValue<string, currencyValue>
}

export interface currencyValue{
  code: string,
  value: number,
}


@Injectable()
export class GroupService {
  constructor(private readonly http: HttpClient) {
  }

  async getMyGroups() {
    const call = this.http.get<GroupCard[]>(environment.apiBaseUrl + "/mygroups")
    return await firstValueFrom<GroupCard[]>(call);
  }

  create(value: CreateGroup) {
    const formData = new FormData();
    Object.entries(value).forEach(([key, value]) =>
      formData.append(key, value)
    );

    return this.http.post<Group>(environment.apiBaseUrl + '/group/create', formData, {
      reportProgress: true,
      observe: 'events'
    });
  }

  createExpense(value: CreateFullExpense) {
    return this.http.post<FullExpense>(environment.apiBaseUrl + '/expense', value);
  }

  createSettle(value: Transaction, groupId: number) {
    return this.http.post<FullExpense>(environment.apiBaseUrl + '/group/' + groupId + '/settle', value)
  }


  async getAllExpenses(groupId: number) {
    const call = this.http.get<FullExpense[]>(environment.apiBaseUrl + '/group/' + groupId + '/expenses');
    return await firstValueFrom<FullExpense[]>(call);
  }

  getGroup(groupId: number) {
    return this.http.get<Group>(environment.apiBaseUrl + '/group/' + groupId);

  }

  async getUserInGroup(groupId: string) {
    const call = this.http.get<UserInGroup[]>(environment.apiBaseUrl + "/group/" + groupId + "/users");
    return await firstValueFrom<UserInGroup[]>(call);
  }


  async getBalances(groupId: number) {
    const call = this.http.get<Balance[]>(environment.apiBaseUrl + '/group/' + groupId + '/balances')
    return await firstValueFrom<Balance[]>(call);
  }

  //gets a list over all transactions to be made before the group is square
  async getAllTransactions(groupId: number) {
    const call = this.http.get<Transaction[]>(environment.apiBaseUrl + '/group/' + groupId + '/transactions');
    return await firstValueFrom<Transaction[]>(call);
  }

  async getMyDebt(groupId: number) {
    const call = this.http.get<Transaction[]>(environment.apiBaseUrl + '/group/' + groupId + '/debt');
    return await firstValueFrom<Transaction[]>(call);
  }

  invite(group_invite: GroupInvitation) {
    return this.http.post<boolean>(environment.apiBaseUrl + '/group/invite', group_invite);
  }

  update(value: GroupUpdate, groupId: number) {
    const formData = new FormData();
    Object.entries(value).forEach(([key, value]) =>
      formData.append(key, value)
    );
    return this.http.put<GroupUpdate>(environment.apiBaseUrl + '/group/' + groupId + '/update', formData, {
      reportProgress: true,
      observe: 'events'
    });
  }


  async getCurrencies() {
    const call = this.http.get<CurrencyList>(environment.apiBaseUrl + "/expense/currency");
    return await firstValueFrom<CurrencyList>(call);
  }

  async deleteExpense(expenseId: number) {
    const call = this.http.delete<boolean>(environment.apiBaseUrl + "/expense/delete/" + expenseId)
    return await firstValueFrom<boolean>(call)
  }

  async isUserGroupOwner(groupId: number) {
    const call = this.http.get<boolean>(environment.apiBaseUrl + "/group/" + groupId + "/owner")
    return await firstValueFrom(call)
  }
}

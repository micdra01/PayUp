import {Component, Input, numberAttribute, OnInit} from '@angular/core';
import {
  CreateExpense,
  CreateFullExpense, CurrencyList, currencyValue,
  FullExpense,
  Group,
  GroupService,
  UserInGroup
} from "../../group/group.service";

import {firstValueFrom, last, Observable} from "rxjs";
import {ActivatedRoute, Router} from "@angular/router";
import {FormBuilder, Validators} from "@angular/forms";
import {ToastController} from "@ionic/angular";
import {KeyValue} from "@angular/common";



@Component({
  selector: 'app-createexpense',
  templateUrl: './createexpense.component.html',
  styleUrls: ['./createexpense.component.scss'],
})
export class CreateexpenseComponent implements OnInit {
  protected readonly Number = Number;
  userInGroup: UserInGroup[] = [];
  id: any;
  usersOnExpense: number[] = [];
  currencyList!: any
  selectedCurrency: KeyValue<string, currencyValue> = {key: 'DKK', value: {code: 'DKK', value: 1}};


  constructor(
    private route: ActivatedRoute,
    private readonly fb: FormBuilder,
    private readonly service: GroupService,
    private readonly toast: ToastController,
    private router: Router
  ) {
  }

  async ngOnInit() {
    await this.getId()
    this.getUsersInGroup(this.id);
  }


  get description() {
    return this.form.controls.description;
  }

  get amount() {
    return this.form.controls.amount;
  }

  get createdDate() {
    return this.form.controls.createdDate;
  }

  async getId() {
    const map = await firstValueFrom(this.route.paramMap)
    this.id = map.get('groupId')
  }

  async getUsersInGroup(groupId: string) {
    this.userInGroup = await this.service.getUserInGroup(groupId)
    this.userInGroup.forEach((u) => this.usersOnExpense.push(u.id))
  }

  async getCurrencies() {
    this.currencyList = await this.service.getCurrencies()
  }

  handleUserSelection(event: CustomEvent) {
    if (event.detail.value !== undefined) {
      this.usersOnExpense = event.detail.value
    }
  }

  form = this.fb.group({

    description: ['', Validators.required],
    amount: [undefined, Validators.required],
    createdDate: [undefined, Validators.required]
  });

  async createExpense() {

    if (this.form.invalid) return

    let currency = Number(this.form.controls.amount.value!) / this.selectedCurrency.value.value;

    var expenseInfo: CreateExpense = {
      groupId: this.id,
      description: this.form.controls.description.value!,
      createdDate: this.form.controls.createdDate.value!,
      isSettle: false,
      amount: currency,


    };

    var fullExpenseInfo: CreateFullExpense = {
      expense: expenseInfo,
      userIdsOnExpense: this.usersOnExpense
    }

    const createdExpense = await firstValueFrom<FullExpense>(this.service.createExpense(fullExpenseInfo as CreateFullExpense))


    await (await this.toast.create({
      message: "Your expense " + createdExpense.expense.description + " was created successfully",
      color: "success",
      duration: 5000
    })).present()
      .then(() => {
        this.router.navigate(['groups/' + this.id])
          .then(() => {
            location.reload();
          });
      });
  }

  async handleCurrencyChange(event: any) {
    this.selectedCurrency = event.detail.value;
  }
}

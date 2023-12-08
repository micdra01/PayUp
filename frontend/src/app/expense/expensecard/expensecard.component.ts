import {Component, Input, OnInit} from '@angular/core';
import {Expense, FullExpense, UserOnExpense} from "../../group/group.service";
import {DatePipe} from "@angular/common";

@Component({
  selector: 'expensecard',
  templateUrl: './expensecard.component.html',
  styleUrls: ['./expensecard.component.scss'],
})
export class ExpensecardComponent  implements OnInit {
  loggedInUser: number = 0
  isPositive: boolean | undefined
  ownShare: number | undefined
  payerImg: string | undefined
  isSettle: boolean | undefined

  constructor() {
  }

  ngOnInit() {
    this.loggedInUser = this.expense.loggedInUser
    this.isSettle = this.expense.expense.isSettle
    this.isPositive = this.getOweOrLent(this.loggedInUser)
    this.ownShare = this.getOwnShare(this.loggedInUser)
    this.getPayer()
  }

  @Input() expense!: FullExpense;

  getOwnShare(id: number) {
    var userOnExpense = this.expense.usersOnExpense.find((u) => u.userId === id)
    if(userOnExpense == undefined) {
      return undefined
    }
    return userOnExpense.amount
  }

  getOweOrLent(id: number) {
    var userOnExpense = this.expense.usersOnExpense.find((u) => u.userId === id)
    if(userOnExpense === undefined) {
      this.isPositive = undefined
    } else this.isPositive = userOnExpense.amount >= 0
    return this.isPositive
  }

  getPayer() {
    var userOnExpense = this.expense.usersOnExpense.find((u) => u.amount >= 0)
    this.payerImg = userOnExpense?.imageUrl
  }
}

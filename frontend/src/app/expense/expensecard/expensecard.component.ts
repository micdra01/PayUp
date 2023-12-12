import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {Expense, FullExpense, GroupService} from "../../group/group.service";
import {IonModal, ToastController} from "@ionic/angular";
import {firstValueFrom} from "rxjs";
import {ActivatedRoute} from "@angular/router";

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
  canDelete: boolean | undefined
  @ViewChild(IonModal) modal: IonModal | undefined;
  public alertButtons = [
    {
      text: 'Cancel',
      role: 'cancel',
    },
    {
      text: 'Delete',
      role: 'confirm',
      handler: () => {
        this.deleteExpense()
      },
    },
  ];

  constructor(
    private route: ActivatedRoute,
    private readonly service: GroupService,
    private readonly toast: ToastController
  ) {
  }

  ngOnInit() {
    this.loggedInUser = this.expense.loggedInUser
    this.isSettle = this.expense.expense.isSettle
    this.isPositive = this.getOweOrLent(this.loggedInUser)
    this.ownShare = this.getOwnShare(this.loggedInUser)
    this.getPayer()
    this.canDelete = this.canLoggedInUserDelete()
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

  cancel() {
    this.modal!.dismiss(null, 'cancel');
  }

  async deleteExpense() {
    const wasDeleted = this.service.deleteExpense(this.expense.expense.id)

    if(await wasDeleted) {
      (await this.toast.create({
        message: "Your expense " + this.expense.expense.description + " was deleted",
        color: "success",
        duration: 5000
      })).present()
        .then(() => {
          this.cancel()
          location.reload()
        });
    }
  }

  canLoggedInUserDelete() {
    return this.loggedInUser === this.expense.expense.userId
  }


}

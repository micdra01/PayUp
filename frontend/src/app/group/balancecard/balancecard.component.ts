import {Component, Input, OnInit} from '@angular/core';
import {Balance, FullExpense, Transaction} from "../group.service";

@Component({
  selector: 'balancecard',
  templateUrl: './balancecard.component.html',
  styleUrls: ['./balancecard.component.scss'],
})
export class BalancecardComponent  implements OnInit {

  payerTransactions: Transaction[] = []
  payeeTransactions: Transaction[] = []

  @Input() transactionList: Transaction[] = []
  @Input() balance!: Balance

  constructor() {
  }

  ngOnInit() {
    this.filterTransActions();//todo should maybe only be done when the balance tab is pushed (a bit heavy method)
  }

  filterTransActions() {
    this.transactionList.forEach( transaction => {
      if (this.balance.userId == transaction.payerId){
        this.payerTransactions.push(transaction);

      }else if (this.balance.userId == transaction.payeeId){
        this.payeeTransactions.push(transaction)
      }
    })
  }
}

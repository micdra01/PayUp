import {Component, Input, OnInit} from '@angular/core';
import {GroupCard} from "../group.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-groupcard',
  templateUrl: './groupcard.component.html',
  styleUrls: ['./groupcard.component.scss'],
})
export class GroupcardComponent  implements OnInit {


  constructor(private router: Router) {
  }

  ngOnInit() {}
  @Input() group!: GroupCard;

  openGroup() {
    this.router.navigate(['groups/'+this.group.id])
  }
}

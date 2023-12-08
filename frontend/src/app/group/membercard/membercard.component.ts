import {Component, Input, OnInit} from '@angular/core';
import {UserInGroup} from "../group.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-membercard',
  templateUrl: './membercard.component.html',
  styleUrls: ['./membercard.component.scss'],
})
export class MembercardComponent  implements OnInit {

  constructor() { }

  ngOnInit() {}
  @Input() member!: UserInGroup;


}

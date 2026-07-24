import { CommonModule } from '@angular/common';
import { Component, input, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { Observable, of } from 'rxjs';
import { IUsers } from '../../models/IUsers';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss'],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, RouterLink, MatIconModule, MatButtonModule],
})
export class UsersComponent {
  readonly users = input.required<IUsers[]>();
  users$!: Observable<IUsers[]>;

  ngOnInit(): void {
    this.users$ = of(this.users());
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Student, StudentsService } from './students.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  title = 'Students';
  students: Student[] = [];
  form: Partial<Student> = { name: '', dept: '' };
  editing: Student | null = null;

  constructor(private api: StudentsService) {}
  ngOnInit(){ this.refresh(); }

  refresh(){ this.api.list().subscribe(s => this.students = s); }

  save(){
    if (!this.form.name || !this.form.dept) return;
    if (this.editing){
      this.api.update({ id: this.editing.id, name: this.form.name!, dept: this.form.dept! })
        .subscribe(() => { this.cancel(); this.refresh(); });
    } else {
      this.api.create({ name: this.form.name!, dept: this.form.dept! })
        .subscribe(() => { this.reset(); this.refresh(); });
    }
  }

  edit(s: Student){ this.editing = s; this.form = { name: s.name, dept: s.dept }; }
  cancel(){ this.editing = null; this.reset(); }
  reset(){ this.form = { name: '', dept: '' }; }
  del(id: number){ this.api.remove(id).subscribe(() => this.refresh()); }
}

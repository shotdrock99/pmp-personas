import { Component, OnInit, Inject } from "@angular/core";
import { FormGroup, FormBuilder } from "@angular/forms";
import { Permiso, Rol } from "src/app/models/rol";
import { PermisosReaderService } from "src/app/services/permisos-reader.service";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-rol-editar",
  templateUrl: "./rol-editar.component.html",
  styleUrls: ["./rol-editar.component.scss"],
})
export class RolEditarComponent implements OnInit {
  editRolForm: FormGroup;
  title: string;
  codigo: string;
  nombre: string;
  descripcion: string;
  permisos: number[];
  permisosRol: Permiso[];

  constructor(
    private permisosReaderService: PermisosReaderService,
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<RolEditarComponent>,
    @Inject(MAT_DIALOG_DATA) private data
  ) {
    this.title = data.title;
    this.codigo = data.codigo;
    this.nombre = data.nombre;
    this.descripcion = data.descripcion;
    this.permisos = data.permisos.map((x) => x.codigo);
  }

  ngOnInit() {
    this.loadPermisosRol();
    this.editRolForm = this.fb.group({
      codigo: [this.codigo, []],
      nombre: [this.nombre, []],
      descripcion: [this.descripcion, []],
      permisos: [this.permisos, []],
    });
  }

  loadPermisosRol() {
    this.permisosReaderService.getPermisos().subscribe((response) => {
      this.permisosRol = response
    });
  }

  edit() {
    let prs: Permiso[] = [];
    this.permisosRol.forEach(p => {
      if (this.permisos.includes(p.codigo)) {
        prs.push(p)
      }
    });
    let response: Rol = {
      codigo: this.editRolForm.value.codigo,
      nombre: this.editRolForm.value.nombre,
      descripcion: this.editRolForm.value.descripcion,
      permisos: prs
    }
    this.dialogRef.close(response);
  }
}

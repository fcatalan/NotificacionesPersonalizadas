import  {Component, OnInit } from '@angular/core';

@Component({
    selector : 'menu',
    templateUrl: 'app/Components/menu/menu.component.html',
})
export class menuComponent implements OnInit{
    titulo: string = 'Bienvenido';

 ngOnInit()
 {
     
 }
}

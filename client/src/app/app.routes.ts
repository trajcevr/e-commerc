import { Routes } from '@angular/router';
import { HomeComponent } from './feature/home/home.component';
import { ShopComponent } from './features/shop/shop.component';
import { ProductDetailsComponent } from './feature/shop/product-details/product-details.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'shop', component: ShopComponent},
    {path: 'shop/:id', component: ProductDetailsComponent},
    {path: '**', redirectTo: '', pathMatch: 'full'},
];

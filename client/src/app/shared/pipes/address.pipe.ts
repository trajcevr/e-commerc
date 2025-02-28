import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';
import { ShippingAddress } from '../models/order';

@Pipe({
  name: 'address',
  standalone: true,
})
export class AddressPipe implements PipeTransform {

  transform(value?: ConfirmationToken["shipping"] | ShippingAddress, ...args: unknown[]): string {
    if (!value) {
      return "Unknown address";
    }
  
    if ('address' in value && value.address) {
      const { line1, line2, city, state, country, postal_code } = value.address;
  
      if (!line1 || !city || !state || !country || !postal_code) {
        return "Incomplete address";
      }
  
      return `${value.name}, ${line1}${line2 ? ", " + line2 : ""}, ${city}, 
        ${state}, ${postal_code}, ${country}`;
    }
  
    if ('line1' in value) {
      const { line1, line2, city, state, country, postalCode } = value;
  
      if (!line1 || !city || !state || !country || !postalCode) {
        return "Incomplete address";
      }
  
      return `${value.name}, ${line1}${line2 ? ", " + line2 : ""}, ${city}, 
        ${state}, ${postalCode}, ${country}`;
    }
  
    return "Unknown address";
  }
  

}

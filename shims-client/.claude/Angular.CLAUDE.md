---
# Angular Signal Forms: Agent Guide

## Overview

Signal Forms use a **model-driven approach**: the form's state and structure come directly from the model you define.  
Form models act as the single source of truth, keeping UI and data in sync automatically.  
This guide covers:

- Form Models
- Designing Models
- Validation
- Form Logic
---

## Form Models

### Creating a Form Model

**login.component.ts**

```ts
import { Component, signal } from '@angular/core';
import { form, FormField } from '@angular/forms/signals';

@Component({
  selector: 'app-login',
  imports: [FormField],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  loginModel = signal({ email: '', password: '' });
  loginForm = form(this.loginModel);
}
```

**login.component.html**

```html
<input type="email" [formField]="loginForm.email" />
<input type="password" [formField]="loginForm.password" />
```

---

### Best Practices

- Always initialize fields (`''` or `null`).
- Use explicit TypeScript types.
- Keep models static and focused.
- Match data types to UI controls.
- Avoid `undefined`.
- Arrays are fine if consistent.

---

### Reading and Updating Values

- Whole form: `this.loginModel()`
- Single field: `loginForm.email().value()`

Update:

```ts
this.userForm.email().value.set('');
this.userForm.age().value.update((age) => age + 1);
```

---

### Two-Way Binding

**user.component.ts**

```ts
@Component({
  selector: 'app-user',
  imports: [FormField],
  templateUrl: './user.component.html',
})
export class UserComponent {
  userModel = signal({ name: '' });
  userForm = form(this.userModel);

  setName(name: string) {
    this.userForm.name().value.set(name);
  }
}
```

**user.component.html**

```html
<input [formField]="userForm.name" />
<button (click)="setName('Bob')">Set Name</button>
<p>Current name: {{ userModel().name }}</p>
```

---

## Designing Your Form Model

### Form vs Domain Models

- **Form model**: UI-friendly, raw input.
- **Domain model**: optimized for business logic/storage.
- Translate between them using `linkedSignal`.

---

### Validation and Structure

- Group related fields.
- Use static structures for conditional fields.
- Hide/disable unused fields.
- Dynamic structures only for arrays or atomic fields.

---

### Translating Between Models

Domain → Form:

```ts
linkedSignal({
  source: this.domainModel,
  computation: (domainModel) =>
    domainModel ? domainModelToFormModel(domainModel) : EMPTY_FORM_MODEL,
});
```

Form → Domain:

```ts
submit(this.myForm, async () => {
  await this.dataService.update(formModelToDomainModel(this.myForm.value()));
});
```

---

## Validation

### Basics

Validation rules are defined in the schema function:

```ts
loginForm = form(this.loginModel, (schemaPath) => {
  required(schemaPath.email, { message: 'Email is required' });
  email(schemaPath.email, { message: 'Enter a valid email address' });
});
```

---

### Built-In Rules

- `required()`
- `email()`
- `min()` / `max()`
- `minLength()` / `maxLength()`
- `pattern()`
- `applyEach()` for arrays

---

### Custom Validation

```ts
validate(schemaPath.website, ({ value }) => {
  if (!value().startsWith('https://')) {
    return { kind: 'https', message: 'URL must start with https://' };
  }
  return null;
});
```

Cross-field:

```ts
validate(schemaPath.confirmPassword, ({ value, valueOf }) => {
  if (value() !== valueOf(schemaPath.password)) {
    return { kind: 'passwordMismatch', message: 'Passwords do not match' };
  }
  return null;
});
```

---

### Async Validation

```ts
validateHttp(schemaPath.username, {
  request: ({ value }) => `/api/check-username?username=${value()}`,
  onSuccess: (response) =>
    response.taken ? { kind: 'usernameTaken', message: 'Username is taken' } : null,
  onError: () => ({
    kind: 'networkError',
    message: 'Could not verify username',
  }),
});
```

---

### Schema Libraries

```ts
import * as z from 'zod';

const userSchema = z.object({
  email: z.string().email(),
  password: z.string().min(8),
});

validateStandardSchema(schemaPath, userSchema);
```

---

## Form Logic

### Disabled Fields

```ts
disabled(schemaPath.couponCode, ({ valueOf }) => valueOf(schemaPath.total) < 50);
```

Return string for reasons:

```ts
disabled(schemaPath.couponCode, ({ valueOf }) =>
  valueOf(schemaPath.total) < 50 ? 'Order must be $50 or more' : false,
);
```

---

### Hidden Fields

```ts
hidden(schemaPath.publicUrl, ({ valueOf }) => !valueOf(schemaPath.isPublic));
```

---

### Readonly Fields

```ts
readonly(schemaPath.username);
readonly(schemaPath.title, ({ valueOf }) => valueOf(schemaPath.isLocked));
```

---

### Hidden vs Disabled vs Readonly

| Feature                | hidden() | disabled() | readonly() |
| ---------------------- | -------- | ---------- | ---------- |
| Visible in UI          | No       | Yes        | Yes        |
| Focus/select           | No       | No         | Yes        |
| Included in submission | No       | No         | Yes        |

---

### Debouncing

```ts
debounce(schemaPath.query, 300);
```

Custom:

```ts
debounce(schemaPath.query, () => new Promise<void>((resolve) => setTimeout(resolve, 500)));
```

---

### Metadata

Pre-defined: REQUIRED, MIN, MAX, MIN_LENGTH, MAX_LENGTH, PATTERN.
Custom:

```ts
export const PLACEHOLDER = createMetadataKey<string>();
metadata(schemaPath.email, PLACEHOLDER, () => 'user@example.com');
```

Accumulating:

```ts
export const HINTS = createMetadataKey<string, string[]>(MetadataReducer.list());
metadata(schemaPath.password, HINTS, () => 'At least 8 characters');
```

---

### Combining Rules

```ts
disabled(schemaPath.promoCode, ({ valueOf }) => !valueOf(schemaPath.hasAccount));
hidden(schemaPath.promoCode, ({ valueOf }) => valueOf(schemaPath.subscriptionType) === 'free');
debounce(schemaPath.promoCode, 300);
metadata(schemaPath.promoCode, PLACEHOLDER, () => 'Enter promo code');
```

---

### Conditional Logic

```ts
applyWhen(
  schemaPath,
  ({ valueOf }) => valueOf(schemaPath.country) === 'US',
  (schemaPath) => {
    required(schemaPath.zipCode);
    pattern(schemaPath.zipCode, /^\d{5}(-\d{4})?$/);
  },
);
```

---

### Reusable Schema Functions

```ts
function emailFieldConfig(path: SchemaPath<string>) {
  debounce(path, 300);
  metadata(path, PLACEHOLDER, () => 'user@example.com');
  maxLength(path, 255);
}
```

---

## Key Takeaways

- **Form Models**: single source of truth.
- **Designing Models**: static, UI-friendly, translate to domain models.
- **Validation**: schema-based, built-in + custom + async.
- **Form Logic**: control availability, visibility, performance, and metadata.
- Combine rules and extract reusable schema functions for maintainability.

---

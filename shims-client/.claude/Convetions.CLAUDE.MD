## Modules

## Modules

When asked to create a module

- Create a folder with the name of the module
- Create an HTTP service file
- Create a folder called `components`
- Create a route file with the name of the module

  ```ts
      import { Routes } from '@angular/router';
      export const <MODULENAME>Route: Routes = [

      ]
  ```

- in the `app.routes.ts` file, import the <MODULENAME>Route
  ```ts
      {
      path: 'schemes',
      loadChildren: () => import('..<path-to-the-route>')
          .then(x => <route-name>)
  }
  ```

## Components

When asked to create a component

- Create a components folder in the module if it does not exists
- Use the `np mcp` instructions to create the component

## Services

### Http Services

- Generate a file with the name provided
- Inject the HttpClient
- Import the Environment file

```ts
@Injectable({providedIn: 'root'})
export class <ServiceName>Service {
    private http = inject(HttpClient);

    list(): Observable<ReturnType>{
        return this.http.get<ReturnType>(`${environment.AppUrl}<path>`)
    }
}
```

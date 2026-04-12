# HooperMediaClient

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.2.7.

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Docker

This application is configured to run as an Angular SSR Node container.

Run commands from the project root:

```bash
Client/hooper-media-client
```

Build and start the client container (fresh build):

```bash
docker compose up --build
```

The app will be available at `http://localhost:56165`.

If you see stale UI from older builds, run a full no-cache rebuild:

```bash
docker compose down --remove-orphans
docker compose build --no-cache
docker compose up -d --force-recreate
```

Stop and remove the container and network:

```bash
docker compose down
```

Optional: run in detached mode:

```bash
docker compose up --build -d
```

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.

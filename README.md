# Dotnet JWT Authentication Project
An application to demostrate JWT based authentication for REST APIs.

---

# Requirements
- docker

# Run

### 1. Docker build and run
```sh
cd AuthService/app.auth

docker build . --tag authservice:1.0.1 && docker run --rm -d -p 8080:8080 -p 8081:8081 --name authservice authservice:1.0.1
```

### 2. Docker Hub
Directly run from Dockerhub
```sh
docker run --rm -d -p 8080:8080 -p 8081:8081 --name authservice unarybinaryternary/authservice:1.0.1
```
Then access: [http://127.0.0.1:8080/swagger](http://127.0.0.1:8080/swagger) in your browser.

### 3. Run locally with dotnet 8.0
```sh
cd AuthService/app.auth

dotnet build

dotnet run
```

Then access: [http://127.0.0.1:8080/swagger](http://127.0.0.1:8080/swagger) in your browser.

# API Testing Commands

## Sign up
```sh
curl -X 'POST' 'http://localhost:8080/api/auth/signup' -H 'accept: */*' -H 'Content-Type: application/json' -d '{"email": "user@example.com", "password": "Strong@123"}'

```

## Sign in
```sh
curl -X 'POST' 'http://localhost:8080/api/auth/signin' -H 'accept: */*' -H 'Content-Type: application/json' -d '{"email": "user@example.com", "password": "Strong@123"}'
```

## Get user details with the token
```sh
curl -X 'POST' 'http://localhost:8080/api/auth/get-user-details' -H 'accept: */*' -H 'Content-Type: application/json' -d '{"email": "user@example.com", "accessToken":"<access_token>"}'
```

## Revoke Token
```sh
curl -X 'POST' 'http://localhost:8080/api/auth/revoke-token?refreshToken=<refresh_token>' -H 'accept: */*'
```

## Refresh Token (get a new access token)
```sh
curl -X 'POST' 'http://localhost:8080/api/auth/refresh-access-token?refreshToken=<refresh_token>' -H 'accept: */*'
```


# Docker commands

## Build and tag the image
```sh
docker build . --tag authservice:1.0.1
```

```sh
docker run --rm -d -p 8080:8080 -p 8081:8081 --name authservice authservice:1.0.1
```
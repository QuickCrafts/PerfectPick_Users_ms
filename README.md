
# Users Microservice

Users CRUD and authentication management.

---
<br />

## API Reference

### Registration

#### Sign up User

Create new user on database with only required data and send an email to verified the user. Returns user id generated.

```http
  POST /users
```

```typescript
// Body interface
interface Create_User{
  firstname: string
  lastname: string // User last name
  email: string // User email - Check if email is valid
  password: string // User password to encrypt
  role: 'READER' | 'ADMIN'
}
```

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | Returns user id|
| `400` | `error` | "Guard failed" |
| `400` | `error` | "Email already registered" |
| `500` | `error` | Any other error message|

```typescript
// Response interface
interface Create_User_Response{
  id: number // User id
}
```

#### Sign up User with google

Create new user on database using google account services. Returns user id generated. Verification email not needed. The user is a classified as "READER" automatically.

```http
  POST /users/google
```

```typescript
// Body interface
interface Create_User{
  google_token: string
}
```

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | Returns user id|
| `400` | `error` | "Guard failed" |
| `400` | `error` | "Email already registered" |
| `500` | `error` | Any other error message|

```typescript
// Response interface
interface Create_User_Response{
  id: number // User id
}
```

#### Sign up User with facebook

Create new user on database using facebook account services. Returns user id generated. Verification email not needed. The user is a classified as "READER" automatically.

```http
  POST /users/facebook
```

```typescript
// Body interface
interface Create_User{
  facebook_token: string
}
```

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | Returns user id|
| `400` | `error` | "Guard failed" |
| `400` | `error` | "Email already registered" |
| `500` | `error` | Any other error message|

```typescript
// Response interface
interface Create_User_Response{
  id: number // User id
}
```

#### Confirm user

Verify new user account.

```http
  POST /users/verify
```

```typescript
// Body interface
interface {
  email: string
  token: string
}
```

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | "User verified."|
| `404` | `error` | "User not found"|
| `400` | `error` | "Email not provided" |
| `403` | `error` | "Invalid token" |
| `500` | `error` | Any other error message|

### Set up

#### Update user

Update user information, email restricted.

```http
  PUT /users/${id}
```

```typescript
// Body interface
interface Update_User{
  firstname?: string
  lastname?: string // User last name
  avatar?: string // String Base64 with avatar image
  birthdate?: string // String with the timestamp
  gender?: 'M' | 'F' | 'O' | 'P' // User gender coded
  country?: number // Country Id
}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | **Required**. User id |

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | "User updated"|
| `404` | `error` | "User not found"|
| `400` | `error` | "Id not provided" |
| `400` | `error` | "Guard failed" |
| `500` | `error` | Any other error message|

#### Complete user set up

User has been completed his setup.

```http
  POST /users/setup/${id}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | **Required**. User id |

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | "User setup complete attribute updated."|
| `404` | `error` | "User not found"|
| `400` | `error` | "Email not provided" |
| `500` | `error` | Any other error message|

### Auth

#### Token secure

Verify is a valid session token.

```http
  GET /users/setup/${token}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `token` | `string` | **Required**. Session token |

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `200` | `success` | "Valid token."|
| `403` | `error` | "Invalid token."|
| `400` | `error` | "Token not provided" |
| `500` | `error` | Any other error message|

#### Log in User with email

Generate access token on using password and user email.

```http
  POST /users/login
```

```typescript
// Body interface
interface {
  email: string
  password: string
}
```

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | return session token|
| `401` | `error` | "User not found" |
| `400` | `error` | "Incorrect email or password" |
| `500` | `error` | Any other error message|

```typescript
// Response interface
interface Response_Login{
  token: string // user session token
}
```

#### Log in User with google

Generate access token on using google account.

```http
  POST /users/login
```

```typescript
// Body interface
interface {
  google_token: string
}
```

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | return session token|
| `401` | `error` | "User not found" |
| `400` | `error` | "Guard failed" |
| `500` | `error` | Any other error message|

```typescript
// Response interface
interface Response_Login{
  token: string // user session token
}
```

#### Log in User with facebook

Generate access token on using facebook account.

```http
  POST /users/login
```

```typescript
// Body interface
interface {
  facebook_token: string
}
```

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | return session token|
| `401` | `error` | "User not found" |
| `400` | `error` | "Guard failed" |
| `500` | `error` | Any other error message|

```typescript
// Response interface
interface Response_Login{
  token: string // user session token
}
```

### User Management

#### Delete user

Soft delete all user information: User instance except for user id, wishlists, tokens and recommendations. (Relation rating no).

```http
  DELETE /users/${id}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | **Required**. User id |

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `204` | `success` |No content|
| `404` | `error` | "User not found"|
| `400` | `error` | "Id not provided" |
| `500` | `error` | Any other error message|


#### Get user

Get essential user information.

```http
  GET /users/${id}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | **Required**. User id |

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `200` | `success` | Body response with user information|
| `404` | `error` | "User not found"|
| `400` | `error` | "Id not provided" |
| `500` | `error` | Any other error message|

```typescript
// Body interface
interface Country{
  id: number
  name: string // English name
  code_2: string //ISO 3166-1 alpha-2
  code_3: string //ISO 3166-1 alpha-3
}

interface Response_Get_User{
  id: string // User id
  firstname: string
  lastname: string // User last name
  avatar_url?: string // Url of avatar image
  birthdate?: string // String with the timestamp
  gender?: 'M' | 'F' | 'O' | 'P' // User gender coded
  country?: Country // Country information
  created_time: string // String with the timestamp
  email: string
  verified: boolean
  setup: boolean
  role: 'READER' | 'ADMIN'
}
```

#### Get users

Get essential user information of each user in the system filtered by gender, nationality and age range.

```http
  GET /users
```

| Query Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `gender` | `enum("M", "F", "O", "P")` | Gender to filter |
| `country` | `int`  | Country id to filter|
| `low_age` | `int`  | Age range lower limit to filter|
| `hight_age` | `int`  | Age range superior limit to filter|

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `200` | `success` | Body response with user information|
| `404` | `error` | "User not found"|
| `400` | `error` | "Id not provided" |
| `500` | `error` | Any other error message|

```typescript
interface Country{
  id: number
  name: string // English name
  code_2: string //ISO 3166-1 alpha-2
  code_3: string //ISO 3166-1 alpha-3
}

interface User{
  id: string // User id
  firstname: string
  lastname: string // User last name
  avatar_url?: string // Url of avatar image
  birthdate?: string // String with the timestamp
  gender?: 'M' | 'F' | 'O' | 'P' // User gender coded
  country?: Country // Country information
  created_time: string // String with the timestamp
  email: string
  verified: boolean
  setup: boolean
  role: 'READER' | 'ADMIN'
}

// Body interface
interface Response_Get_Users{
  users: User[]
}
```

### Nationality

#### Get Country

Get country information.

```http
  GET /countries/${id}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | **Required**. Country id |

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `200` | `success` | Body response with country information|
| `404` | `error` | "Country not found"|
| `400` | `error` | "Id not provided" |
| `500` | `error` | Any other error message|

```typescript
// Body interface
interface Response_Get_Country{
  id: number
  name: string // English name
  code_2: string //ISO 3166-1 alpha-2
  code_3: string //ISO 3166-1 alpha-3
}
```

#### Get Countries

Get country information of each country in the system.

```http
  GET /countries
```

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `200` | `success` | Body response with countries information|
| `404` | `error` | "Country not found"|
| `400` | `error` | "Id not provided" |
| `500` | `error` | Any other error message|

```typescript
interface Country{
  id: number
  name: string // English name
  code_2: string //ISO 3166-1 alpha-2
  code_3: string //ISO 3166-1 alpha-3
}

// Body interface
interface Get_Countries{
  countries: Country[]
}
```

#### Create Country

Create new country information.

```http
  POST /countries
```

```typescript
// Body interface
interface Create_Country{
  name: string // English name
  code_2: string //ISO 3166-1 alpha-2
  code_3: string //ISO 3166-1 alpha-3
}
```

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | Returns country id|
| `400` | `error` | "Guard failed" |
| `500` | `error` | Any other error message|

```typescript
// Response interface
interface Create_Country_Response{
  id: number // User id
}
```

#### Update Country

Update new country information.

```http
  PUT /countries/${id}
```

```typescript
// Body interface
interface Update_Country{
  name?: string // English name
  code_2?: string //ISO 3166-1 alpha-2
  code_3?: string //ISO 3166-1 alpha-3
}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | **Required**. Country id |

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | "Country updated"|
| `404` | `error` | "Country not found" |
| `400` | `error` | "Guard failed" |
| `500` | `error` | Any other error message|

#### Delete Country

Delete country information.

```http
  DELETE /countries
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | **Required**. Country id |

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | "Country deleted."|
| `404` | `error` | "Country not found" |
| `400` | `error` | "Id not provided." |
| `500` | `error` | Any other error message|

#### Initialization and update of countries

Get countries information from [API](https://restcountries.com/) to init PerfectPick country database or to update with new missing countries.

```http
  PUT /countries
```

| Response Status | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `201` | `success` | "Countries updated."|
| `500` | `error` | Any other error message|


---
<br />
<br />
<br />


## Deployment

To deploy this project run

[//]: <> (@todo correct)

```bash
  npm run deploy
```

## Run Locally

Clone the project

```bash
  git clone https://github.com/QuickCrafts/PerfectPick_User_ms.git
```

Go to the project directory

```bash
  cd PerfectPick_User_ms
```
[//]: <> (@todo correct all)

Install dependencies

```bash
  npm install
```

Start the server

```bash
  npm 
```

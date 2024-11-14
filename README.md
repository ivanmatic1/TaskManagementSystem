# **Project Title**

A brief description of what this project does and who it's for


## **API Documentation**

### **Auth**
#### **1. Register User**
**POST** `/api/auth/register`

**Request Body:**
```
{
  "username": "user123",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "password": "P@ssw0rd"
}
```
**Response:**
```
{
  "message": "User has been registered and assigned the User role."
}
```

#### **2. Login User**
**POST** `/api/auth/login`

**Request Body:**
```
{
  "email": "user@example.com",
  "password": "P@ssw0rd"
}

```
**Response:**
```
{
  "token": "your-jwt-token"
}
```
### **Project**
#### **1. Create Project**

```POST /api/project/create```

**Request Body:**

```
{
  "name": "Project X",
  "description": "A description of Project X",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-12-31T00:00:00Z",
  "memberEmails": ["email1@example.com", "email2@example.com"]
}
```

 **Response:**

 ```
 {
  "id": 1,
  "name": "Project X",
  "description": "A description of Project X",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-12-31T00:00:00Z",
  "ownerId": "userId",
  "ownerName": "User1",
  "memberIds": ["memberId1", "memberId2"]
}
```
#### 2. **Update Project**

```PUT /api/project/update/{id}```

**Request Body**

```
{
  "name": "Updated Project X",
  "description": "Updated description",
  "startDate": "2024-02-01T00:00:00Z",
  "endDate": "2025-12-31T00:00:00Z",
  "memberEmails": ["email1@example.com", "email3@example.com"]
}
 ```

**Response**
 ```
 {
  "id": 1,
  "name": "Updated Project X",
  "description": "Updated description",
  "startDate": "2024-02-01T00:00:00Z",
  "endDate": "2025-12-31T00:00:00Z",
  "ownerId": "userId",
  "ownerName": "User1",
  "memberIds": ["memberId1", "memberId3"]
}
```
#### 3. **Delete Project**

```DELETE /api/project/delete/{id}```

**Response**
```Status Code: 204 No Content```

#### 4. **Get Project by ID**

```GET /api/project/getproject/{id}```

**Response**

```
{
  "id": 1,
  "name": "Project X",
  "description": "A description of Project X",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-12-31T00:00:00Z",
  "ownerId": "userId",
  "ownerName": "User1",
  "memberIds": ["memberId1", "memberId2"]
}

```
#### 5. **Get All User Projects**

```GET /api/project/getprojects```

**Response**

```
[
  {
    "id": 1,
    "name": "Project X",
    "description": "A description of Project X",
    "startDate": "2024-01-01T00:00:00Z",
    "endDate": "2024-12-31T00:00:00Z",
    "ownerId": "userId",
    "ownerName": "User1",
    "memberIds": ["memberId1", "memberId2"]
  },
  {
    "id": 2,
    "name": "Project Y",
    "description": "A description of Project Y",
    "startDate": "2024-06-01T00:00:00Z",
    "endDate": "2024-12-31T00:00:00Z",
    "ownerId": "userId",
    "ownerName": "User1",
    "memberIds": ["memberId1"]
  }
]

```

#### 6. **Add Members to Project**

```POST /api/project/getproject/{id}```

**Request Body**

```
[
  "email1@example.com",
  "email2@example.com"
]
```

**Response**

```
{
  "message": "Members added successfully."
}

```

#### 7. **Remove Members from Project**

```Delete /api/project/removemember/{projectId}```

**Request Body**

```
[
  "email1@example.com",
  "email2@example.com"
]
```

**Response**

```
{
  "message": "Members removed successfully."
}
```
#### 8. **Get All Users in Project**

```GET /api/project/{projectId}/users```

**Response**

```
[
  {
    "firstName": "User",
    "lastName": "One",
    "userName": "user1",
    "email": "user1@example.com",
    "roles": ["Admin"]
  },
  {
    "firstName": "User",
    "lastName": "Two",
    "userName": "user2",
    "email": "user2@example.com",
    "roles": ["Member"]
  }
]
```


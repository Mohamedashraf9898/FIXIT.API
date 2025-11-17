# ?? SCHEDULING API - QUICK REFERENCE GUIDE

## Base URLs

```
Availability: /api/availability
Time Off: /api/timeoff
```

---

## Availability Endpoints

### 1. Get All Availabilities for a Craftsman
```
GET /api/availability/craftsman/{craftsmanId}

Example: GET /api/availability/craftsman/1

Response:
{
  "success": true,
  "data": [
    {
      "id": 1,
      "dayOfWeek": "Monday",
      "dayName": "Monday",
      "startTime": "09:00:00",
      "endTime": "17:00:00",
      "startTimeFormatted": "09:00",
      "endTimeFormatted": "17:00",
      "isAvailable": true
    }
  ],
  "message": "Availabilities retrieved successfully"
}
```

---

### 2. Get Availability for Specific Day
```
GET /api/availability/craftsman/{craftsmanId}/day/{dayOfWeek}

Example: GET /api/availability/craftsman/1/day/1

Note: DayOfWeek values: 
  0 = Sunday
  1 = Monday
  2 = Tuesday
  3 = Wednesday
  4 = Thursday
  5 = Friday
  6 = Saturday
```

---

### 3. Create New Availability
```
POST /api/availability
Content-Type: application/json

{
  "craftsManId": 1,
  "dayOfWeek": 1,
  "startTime": "09:00",
  "endTime": "17:00",
  "isAvailable": true
}

Response (201 Created):
{
  "success": true,
  "data": {
    "id": 1,
    "dayOfWeek": "Monday",
    "dayName": "Monday",
    "startTime": "09:00:00",
    "endTime": "17:00:00",
    "startTimeFormatted": "09:00",
    "endTimeFormatted": "17:00",
    "isAvailable": true
  },
  "message": "Availability created successfully"
}
```

---

### 4. Update Availability
```
PUT /api/availability/{id}
Content-Type: application/json

{
  "startTime": "08:00",
  "endTime": "18:00",
  "isAvailable": true
}

Response (200 OK):
{
  "success": true,
  "data": { ... },
  "message": "Availability updated successfully"
}
```

---

### 5. Delete Availability
```
DELETE /api/availability/{id}

Response (200 OK):
{
  "success": true,
  "message": "Availability deleted successfully"
}
```

---

### 6. Check Availability on Specific Day
```
GET /api/availability/check-availability/{craftsmanId}/{dayOfWeek}

Example: GET /api/availability/check-availability/1/1

Response:
{
  "success": true,
  "data": {
    "isAvailable": true
  },
  "message": "Availability check completed"
}
```

---

## Time Off Endpoints

### 1. Get All Time Offs for Craftsman
```
GET /api/timeoff/craftsman/{craftsmanId}

Response:
{
  "success": true,
  "data": [
    {
      "id": 1,
      "craftsManId": 1,
      "startDate": "2025-12-20T00:00:00",
      "endDate": "2025-12-27T23:59:59",
      "type": 0,
      "typeDescription": "Vacation",
      "reason": "Family vacation",
      "isApproved": true,
      "createdAt": "2025-11-15T10:30:00",
      "durationDays": 8
    }
  ],
  "message": "Time offs retrieved successfully"
}
```

---

### 2. Get Active Time Offs (Current)
```
GET /api/timeoff/craftsman/{craftsmanId}/active

Response: [array of currently active time offs]
```

---

### 3. Get Upcoming Time Offs
```
GET /api/timeoff/craftsman/{craftsmanId}/upcoming?days=30

Optional Query Parameters:
  - days: Number of days to look ahead (default: 30)
```

---

### 4. Create Time Off
```
POST /api/timeoff
Content-Type: application/json

{
  "craftsManId": 1,
  "startDate": "2025-12-20T00:00:00",
  "endDate": "2025-12-27T23:59:59",
  "type": 0,
  "reason": "Family vacation"
}

TimeOffType enum values:
  0 = Vacation
  1 = Sick
  2 = Personal
  3 = Emergency
  4 = Holiday
  5 = Other

Response (201 Created):
{
  "success": true,
  "data": { ... },
  "message": "Time off created successfully"
}
```

---

### 5. Update Time Off
```
PUT /api/timeoff/{id}
Content-Type: application/json

{
  "craftsManId": 1,
  "startDate": "2025-12-20T00:00:00",
  "endDate": "2025-12-27T23:59:59",
  "type": 0,
  "reason": "Updated vacation"
}
```

---

### 6. Delete Time Off
```
DELETE /api/timeoff/{id}

Response (200 OK):
{
  "success": true,
  "message": "Time off deleted successfully"
}
```

---

### 7. Check Time Off on Specific Date
```
GET /api/timeoff/check/{craftsmanId}?date=2025-12-20

Query Parameters:
  - date: Date to check (format: YYYY-MM-DD or ISO 8601)

Response:
{
  "success": true,
  "data": {
    "hasTimeOff": true,
    "date": "2025-12-20"
  },
  "message": "Time off check completed"
}
```

---

## Error Responses

### Validation Error (400 Bad Request)
```json
{
  "statusCode": 400,
  "message": "StartTime must be before EndTime"
}
```

### Not Found Error (404)
```json
{
  "statusCode": 404,
  "message": "CraftsMan with id 999 was not found."
}
```

### Server Error (500)
```json
{
  "statusCode": 500,
  "message": "An unexpected error occurred"
}
```

---

## Example Workflow

### 1. Set Up Weekly Availability
```bash
# Monday 9AM - 5PM
POST /api/availability
{
  "craftsManId": 1,
  "dayOfWeek": 1,
  "startTime": "09:00",
  "endTime": "17:00"
}

# Tuesday 9AM - 5PM
POST /api/availability
{
  "craftsManId": 1,
  "dayOfWeek": 2,
  "startTime": "09:00",
  "endTime": "17:00"
}

# ... repeat for other days
```

### 2. Mark Day Off
```bash
PUT /api/availability/1
{
  "startTime": "09:00",
  "endTime": "17:00",
  "isAvailable": false
}
```

### 3. Create Vacation
```bash
POST /api/timeoff
{
  "craftsManId": 1,
  "startDate": "2025-12-20T00:00:00",
  "endDate": "2025-12-27T23:59:59",
  "type": 0,
  "reason": "Christmas vacation"
}
```

### 4. Check Availability
```bash
GET /api/availability/check-availability/1/1
# Returns: { "isAvailable": true }

GET /api/timeoff/check/1?date=2025-12-25
# Returns: { "hasTimeOff": true, "date": "2025-12-25" }
```

---

## Validation Rules

### Time Format
- Must be in HH:mm format (24-hour)
- Valid examples: "09:00", "13:30", "23:59"
- Invalid examples: "9:0", "25:00", "9am"

### Date Format
- ISO 8601 format: "2025-12-20T00:00:00"
- Or: "2025-12-20T23:59:59"
- Timezone: UTC

### DayOfWeek
- Sunday = 0
- Monday = 1
- Tuesday = 2
- Wednesday = 3
- Thursday = 4
- Friday = 5
- Saturday = 6

---

## HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request (validation error) |
| 404 | Not Found |
| 500 | Server Error |

---

## Testing with Postman

### Collection Template
```json
{
  "info": {
    "name": "FIXIT Scheduling API"
  },
  "item": [
    {
      "name": "Get Availabilities",
      "request": {
        "method": "GET",
        "url": "{{baseUrl}}/api/availability/craftsman/1"
      }
    },
    {
      "name": "Create Availability",
      "request": {
        "method": "POST",
        "url": "{{baseUrl}}/api/availability",
        "body": {
          "craftsManId": 1,
          "dayOfWeek": 1,
          "startTime": "09:00",
          "endTime": "17:00"
        }
      }
    }
  ]
}
```

---

## Implementation Notes

- ? All endpoints are async
- ? Full input validation
- ? Proper error handling
- ? Consistent response format
- ? RESTful design
- ? Database optimized queries
- ? Proper HTTP status codes
- ? Clear error messages

---

**Last Updated:** 2025-11-15
**Status:** ? Ready for Use

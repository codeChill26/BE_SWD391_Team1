# API Documentation - Flow A: Franchise Store ??t Hàng

## Overview
API cho phép nhân viên c?a hàng franchise ??t hàng t? b?p trung tâm.

## Base URL
```
http://localhost:5000/api
```

## Flow ??t Hàng

### Step 1: Ki?m tra t?n kho c?a b?p trung tâm

**Endpoint:** `GET /api/Inventory/kitchen/{kitchenId}`

**Description:** L?y danh sách s?n ph?m và s? l??ng t?n kho t?i b?p trung tâm

**Request:**
```http
GET /api/Inventory/kitchen/KITC_20260128_j2n7
```

**Response:** `200 OK`
```json
[
  {
    "productId": "PROD_20260128_a3x9",
    "productName": "B?t mì",
    "categoryName": "Nguyên li?u",
    "unit": "kg",
    "availableQuantity": 500.00
  },
  {
    "productId": "PROD_20260128_b4y8",
    "productName": "???ng",
    "categoryName": "Nguyên li?u",
    "unit": "kg",
    "availableQuantity": 200.50
  }
]
```

---

### Step 2: T?o ??n ??t hàng

**Endpoint:** `POST /api/Order`

**Description:** 
- Ch?n nhi?u s?n ph?m
- Nh?p s? l??ng c?a t?ng s?n ph?m
- Ch?n th?i gian nh?n hàng
- T?o ??n v?i tr?ng thái "pending" (Ch? b?p ti?p nh?n)

**Request:**
```http
POST /api/Order
Content-Type: application/json

{
  "storeId": "STOR_20260128_p5w8",
  "kitchenId": "KITC_20260128_j2n7",
  "expectedDeliveryAt": "2026-01-30T10:00:00",
  "items": [
    {
      "productId": "PROD_20260128_a3x9",
      "quantity": 50.0
    },
    {
      "productId": "PROD_20260128_b4y8",
      "quantity": 25.5
    }
  ]
}
```

**Response:** `201 Created`
```json
{
  "message": "??n hàng ???c t?o thành công v?i tr?ng thái 'Ch? b?p ti?p nh?n'",
  "order": {
    "id": "ORD_20260128_k7m2",
    "storeId": "STOR_20260128_p5w8",
    "storeName": "C?a hàng Qu?n 1",
    "kitchenId": "KITC_20260128_j2n7",
    "kitchenName": "B?p Trung Tâm TP.HCM",
    "status": "pending",
    "createdAt": "2026-01-28T14:30:00",
    "expectedDeliveryAt": "2026-01-30T10:00:00",
    "items": [
      {
        "productId": "PROD_20260128_a3x9",
        "productName": "B?t mì",
        "unit": "kg",
        "quantity": 50.0
      },
      {
        "productId": "PROD_20260128_b4y8",
        "productName": "???ng",
        "unit": "kg",
        "quantity": 25.5
      }
    ]
  }
}
```

---

### Step 3: Xem chi ti?t ??n hàng

**Endpoint:** `GET /api/Order/{orderId}`

**Description:** L?y thông tin chi ti?t ??n hàng ?ã t?o

**Request:**
```http
GET /api/Order/ORD_20260128_k7m2
```

**Response:** `200 OK`
```json
{
  "id": "ORD_20260128_k7m2",
  "storeId": "STOR_20260128_p5w8",
  "storeName": "C?a hàng Qu?n 1",
  "kitchenId": "KITC_20260128_j2n7",
  "kitchenName": "B?p Trung Tâm TP.HCM",
  "status": "pending",
  "createdAt": "2026-01-28T14:30:00",
  "expectedDeliveryAt": "2026-01-30T10:00:00",
  "items": [
    {
      "productId": "PROD_20260128_a3x9",
      "productName": "B?t mì",
      "unit": "kg",
      "quantity": 50.0
    }
  ]
}
```

---

### Xem t?t c? ??n hàng c?a c?a hàng

**Endpoint:** `GET /api/Order/store/{storeId}`

**Description:** L?y danh sách t?t c? ??n hàng c?a m?t c?a hàng

**Request:**
```http
GET /api/Order/store/STOR_20260128_p5w8
```

**Response:** `200 OK`
```json
[
  {
    "id": "ORD_20260128_k7m2",
    "storeId": "STOR_20260128_p5w8",
    "storeName": "C?a hàng Qu?n 1",
    "status": "pending",
    "createdAt": "2026-01-28T14:30:00",
    "expectedDeliveryAt": "2026-01-30T10:00:00",
    "items": [...]
  },
  {
    "id": "ORD_20260127_x3y4",
    "storeId": "STOR_20260128_p5w8",
    "storeName": "C?a hàng Qu?n 1",
    "status": "completed",
    "createdAt": "2026-01-27T09:00:00",
    "expectedDeliveryAt": "2026-01-28T14:00:00",
    "items": [...]
  }
]
```

---

## Order Status Flow

```
pending         -> Ch? b?p ti?p nh?n (Order created)
?
confirmed       -> B?p ?ã xác nh?n
?
preparing       -> ?ang chu?n b? hàng
?
ready           -> S?n sàng giao hàng
?
delivering      -> ?ang giao hàng
?
delivered       -> ?ã giao hàng
?
completed       -> Hoàn thành (Store confirmed receipt)
```

## Error Responses

### 400 Bad Request
```json
{
  "message": "??n hàng ph?i có ít nh?t 1 s?n ph?m"
}
```

### 404 Not Found
```json
{
  "message": "Không tìm th?y ??n hàng",
  "error": "Order not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "Có l?i x?y ra khi t?o ??n hàng",
  "error": "Detailed error message..."
}
```

## Testing with Swagger

1. Start the application
2. Navigate to: `https://localhost:7xxx/swagger`
3. Use the interactive UI to test APIs

## Testing with cURL

### 1. Get Kitchen Inventory
```bash
curl -X GET "http://localhost:5000/api/Inventory/kitchen/KITC_20260128_j2n7"
```

### 2. Create Order
```bash
curl -X POST "http://localhost:5000/api/Order" \
  -H "Content-Type: application/json" \
  -d '{
    "storeId": "STOR_20260128_p5w8",
    "kitchenId": "KITC_20260128_j2n7",
    "expectedDeliveryAt": "2026-01-30T10:00:00",
    "items": [
      {
        "productId": "PROD_20260128_a3x9",
        "quantity": 50.0
      }
    ]
  }'
```

### 3. Get Order Details
```bash
curl -X GET "http://localhost:5000/api/Order/ORD_20260128_k7m2"
```

## Notes

- T?t c? endpoint ??u tr? v? JSON
- DateTime format: ISO 8601 (`YYYY-MM-DDTHH:mm:ss`)
- Decimal numbers s? d?ng d?u ch?m (`.`) làm separator
- IDs ???c generate t? ??ng khi t?o ??n hàng

## Architecture

```
Controller Layer (WebAPI)
    ?
Service Layer (BLL)
    ?
Data Access Layer (DAL)
    ?
Database (PostgreSQL)
```

## Database Tables Used

- `orders` - Thông tin ??n hàng
- `order_items` - Chi ti?t s?n ph?m trong ??n
- `franchise_stores` - Thông tin c?a hàng
- `central_kitchens` - Thông tin b?p trung tâm
- `products` - Danh m?c s?n ph?m
- `inventory` - T?n kho t?i các ??a ?i?m

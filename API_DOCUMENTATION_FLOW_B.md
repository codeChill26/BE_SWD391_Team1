# API Documentation - Flow B: Central Kitchen X? Lý & S?n Xu?t

## Overview
API cho phép nhân viên b?p trung tâm xem, ki?m tra, và x? lý ??n hàng t? các c?a hàng.

## Base URL
```
http://localhost:5000/api
```

---

## Flow X? Lý ??n Hàng

### Step 1: Xem danh sách ??n t? các c?a hàng

**Endpoint:** `GET /api/Production/kitchen/{kitchenId}/orders`

**Description:** L?y t?t c? ??n hàng c?n x? lý c?a b?p trung tâm

**Query Parameters:**
- `status` (optional): Filter by status (pending, confirmed, preparing, ready)
- `fromDate` (optional): Filter from date (ISO 8601)
- `toDate` (optional): Filter to date (ISO 8601)

**Request:**
```http
GET /api/Production/kitchen/KITC_20260128_j2n7/orders?status=pending
```

**Response:** `200 OK`
```json
[
  {
    "id": "ORD_20260128_k7m2",
    "storeId": "STOR_20260128_p5w8",
    "storeName": "C?a hàng Qu?n 1",
    "storeAddress": "123 Nguy?n Hu?, Q1, TP.HCM",
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
    ],
    "canProduce": true,
    "productionNote": "?? nguyên li?u ?? s?n xu?t"
  }
]
```

---

### Step 2: H? th?ng Ki?m tra (S? l??ng yêu c?u vs T?n kho)

**Endpoint:** `POST /api/Production/check-capacity`

**Description:** Ki?m tra xem b?p có ?? nguyên li?u ?? s?n xu?t không

**Request:**
```http
POST /api/Production/check-capacity
Content-Type: application/json

{
  "orderId": "ORD_20260128_k7m2",
  "kitchenId": "KITC_20260128_j2n7"
}
```

**Response:** `200 OK`
```json
{
  "orderId": "ORD_20260128_k7m2",
  "canProduce": true,
  "items": [
    {
      "productId": "PROD_20260128_a3x9",
      "productName": "B?t mì",
      "requiredQuantity": 50.0,
      "availableQuantity": 500.0,
      "isSufficient": true,
      "shortageAmount": 0
    },
    {
      "productId": "PROD_20260128_b4y8",
      "productName": "???ng",
      "requiredQuantity": 30.0,
      "availableQuantity": 25.0,
      "isSufficient": false,
      "shortageAmount": 5.0
    }
  ],
  "message": "Thi?u nguyên li?u. Vui lòng ki?m tra l?i t?n kho"
}
```

---

### Step 3a: Ch?p nh?n ??n ? ??a vào k? ho?ch s?n xu?t

**Endpoint:** `PATCH /api/Production/{orderId}/accept`

**Description:** Xác nh?n ??n hàng và ??a vào k? ho?ch s?n xu?t (Status: pending ? confirmed)

**Request:**
```http
PATCH /api/Production/ORD_20260128_k7m2/accept
```

**Response:** `200 OK`
```json
{
  "message": "?ã ch?p nh?n ??n hàng. Tr?ng thái: 'Confirmed'",
  "orderId": "ORD_20260128_k7m2",
  "nextStep": "B?t ??u s?n xu?t khi s?n sàng"
}
```

---

### Step 3b: T? ch?i ??n (Ho?c ch? nguyên li?u)

**Endpoint:** `PATCH /api/Production/{orderId}/reject`

**Description:** T? ch?i ??n hàng vì thi?u nguyên li?u ho?c lý do khác

**Request:**
```http
PATCH /api/Production/ORD_20260128_k7m2/reject
Content-Type: application/json

{
  "reason": "Thi?u nguyên li?u ???ng. C?n ch? nh?p kho"
}
```

**Response:** `200 OK`
```json
{
  "message": "?ã t? ch?i ??n hàng. Lý do: Thi?u nguyên li?u ???ng. C?n ch? nh?p kho",
  "orderId": "ORD_20260128_k7m2"
}
```

---

### Step 4: Th?c hi?n s?n xu?t (Preparing)

**Endpoint:** `POST /api/Production/start`

**Description:** B?t ??u quy trình s?n xu?t (Status: confirmed ? preparing)

**Request:**
```http
POST /api/Production/start
Content-Type: application/json

{
  "orderId": "ORD_20260128_k7m2",
  "kitchenId": "KITC_20260128_j2n7"
}
```

**Response:** `200 OK`
```json
{
  "message": "?ã b?t ??u s?n xu?t. Tr?ng thái: 'Preparing'",
  "orderId": "ORD_20260128_k7m2",
  "nextStep": "Hoàn thành s?n xu?t khi xong"
}
```

**Error Response:** `500 Internal Server Error`
```json
{
  "message": "Order must be confirmed before starting production",
  "error": "Order must be confirmed before starting production"
}
```

---

### Step 5: Hoàn t?t ? Chuy?n sang "S?n sàng giao"

**Endpoint:** `POST /api/Production/complete`

**Description:** Hoàn thành s?n xu?t, s?n ph?m s?n sàng ?? giao (Status: preparing ? ready)

**Request:**
```http
POST /api/Production/complete
Content-Type: application/json

{
  "orderId": "ORD_20260128_k7m2",
  "producedItems": [
    {
      "productId": "PROD_20260128_a3x9",
      "actualQuantity": 50.0,
      "qualityNote": "Ch?t l??ng t?t"
    },
    {
      "productId": "PROD_20260128_b4y8",
      "actualQuantity": 25.5,
      "qualityNote": "??t chu?n"
    }
  ]
}
```

**Response:** `200 OK`
```json
{
  "message": "?ã hoàn thành s?n xu?t. Tr?ng thái: 'Ready' - S?n sàng giao hàng",
  "orderId": "ORD_20260128_k7m2",
  "nextStep": "Chuy?n sang Flow C: Giao hàng"
}
```

---

## Order Status Flow (Kitchen Perspective)

```
pending          ? ??n m?i t? c?a hàng, ch? kitchen x? lý
? (accept)
confirmed        ? Kitchen ?ã xác nh?n, ??a vào k? ho?ch s?n xu?t
? (start)
preparing        ? ?ang s?n xu?t
? (complete)
ready            ? S?n ph?m ?ã xong, s?n sàng giao hàng
? (Flow C)
delivering       ? ?ang giao hàng
?
delivered        ? ?ã giao ??n c?a hàng
?
completed        ? Hoàn thành
```

**Alternative flow:**
```
pending ? rejected (n?u thi?u nguyên li?u ho?c không th? s?n xu?t)
```

---

## Use Cases

### Use Case 1: ??n hàng có ?? nguyên li?u

1. **GET** `/api/Production/kitchen/{kitchenId}/orders` ? Xem ??n pending
2. **POST** `/api/Production/check-capacity` ? Check: `canProduce = true`
3. **PATCH** `/api/Production/{orderId}/accept` ? Ch?p nh?n ??n
4. **POST** `/api/Production/start` ? B?t ??u s?n xu?t
5. **POST** `/api/Production/complete` ? Hoàn thành

### Use Case 2: ??n hàng thi?u nguyên li?u

1. **GET** `/api/Production/kitchen/{kitchenId}/orders` ? Xem ??n pending
2. **POST** `/api/Production/check-capacity` ? Check: `canProduce = false`
3. **PATCH** `/api/Production/{orderId}/reject` ? T? ch?i v?i lý do

---

## Testing with cURL

### 1. Get Kitchen Orders (All pending)
```bash
curl -X GET "http://localhost:5000/api/Production/kitchen/KITC_20260128_j2n7/orders?status=pending"
```

### 2. Check Production Capacity
```bash
curl -X POST "http://localhost:5000/api/Production/check-capacity" \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "ORD_20260128_k7m2",
    "kitchenId": "KITC_20260128_j2n7"
  }'
```

### 3. Accept Order
```bash
curl -X PATCH "http://localhost:5000/api/Production/ORD_20260128_k7m2/accept"
```

### 4. Start Production
```bash
curl -X POST "http://localhost:5000/api/Production/start" \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "ORD_20260128_k7m2",
    "kitchenId": "KITC_20260128_j2n7"
  }'
```

### 5. Complete Production
```bash
curl -X POST "http://localhost:5000/api/Production/complete" \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "ORD_20260128_k7m2",
    "producedItems": [
      {
        "productId": "PROD_20260128_a3x9",
        "actualQuantity": 50.0,
        "qualityNote": "Ch?t l??ng t?t"
      }
    ]
  }'
```

---

## Integration with Flow A

**Flow A (Store)** t?o order v?i status `pending`
?
**Flow B (Kitchen)** x? lý order:
- pending ? confirmed ? preparing ? ready
?
**Flow C (Delivery)** s? x? lý order status `ready`

---

## Notes

- Kitchen ch? x? lý orders có `kitchen_id` trùng v?i kitchen ?ang ??ng nh?p
- H? th?ng t? ??ng check inventory khi g?i `CheckProductionCapacity`
- Order ph?i ?i qua các status theo th? t?: pending ? confirmed ? preparing ? ready
- Không th? skip status ho?c quay ng??c l?i (tr? reject)
- `producedItems` trong CompleteProduction có th? khác v?i order items (quality issues, adjustments)

---

## Error Handling

### 404 Not Found
```json
{
  "message": "Không tìm th?y ??n hàng"
}
```

### 400 Bad Request
```json
{
  "message": "Order must be confirmed before starting production"
}
```

### 500 Internal Server Error
```json
{
  "message": "Có l?i x?y ra khi x? lý ??n hàng",
  "error": "Detailed error message..."
}
```

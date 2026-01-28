-- =========================
-- USERS & ROLES
-- =========================

CREATE TABLE users (
    id VARCHAR(40) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    status VARCHAR(20) NOT NULL
        CHECK (status IN ('ACTIVE', 'INACTIVE', 'BANNED')),
    created_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE roles (
    id VARCHAR(40) PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE user_roles (
    user_id VARCHAR(40) NOT NULL,
    role_id VARCHAR(40) NOT NULL,
    PRIMARY KEY (user_id, role_id),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE
);

-- =========================
-- STORES & KITCHENS
-- =========================

CREATE TABLE franchise_stores (
    id VARCHAR(40) PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    address VARCHAR(255) NOT NULL,
    status VARCHAR(20) NOT NULL
        CHECK (status IN ('ACTIVE', 'INACTIVE'))
);

CREATE TABLE central_kitchens (
    id VARCHAR(40) PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    address VARCHAR(255) NOT NULL
);

-- =========================
-- PRODUCT CATEGORIES & PRODUCTS
-- =========================

CREATE TABLE product_categories (
    id VARCHAR(40) PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE products (
    id VARCHAR(40) PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    category_id VARCHAR(40) NOT NULL,
    type VARCHAR(30) NOT NULL
        CHECK (type IN ('RAW_MATERIAL', 'SEMI_PROCESSED', 'FINISHED')),
    unit VARCHAR(20) NOT NULL
        CHECK (unit IN ('KG', 'GRAM', 'LITER', 'PIECE')),
    FOREIGN KEY (category_id) REFERENCES product_categories(id)
);

-- =========================
-- INVENTORY
-- =========================

CREATE TABLE inventory (
    id VARCHAR(40) PRIMARY KEY,
    product_id VARCHAR(40) NOT NULL,
    location_type VARCHAR(20) NOT NULL
        CHECK (location_type IN ('STORE', 'KITCHEN')),
    location_id VARCHAR(40) NOT NULL,
    quantity DECIMAL(12,2) NOT NULL
        CHECK (quantity >= 0),
    updated_at TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
);

-- =========================
-- ORDERS
-- =========================

CREATE TABLE orders (
    id VARCHAR(40) PRIMARY KEY,
    store_id VARCHAR(40),
    kitchen_id VARCHAR(40),
    status VARCHAR(20) NOT NULL
        CHECK (status IN (
            'PENDING',
            'APPROVED',
            'DELIVERING',
            'COMPLETED',
            'CANCELLED'
        )),
    expected_delivery_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY (store_id) REFERENCES franchise_stores(id),
    FOREIGN KEY (kitchen_id) REFERENCES central_kitchens(id)
);

CREATE TABLE order_items (
    order_id VARCHAR(40) NOT NULL,
    product_id VARCHAR(40) NOT NULL,
    quantity DECIMAL(12,2) NOT NULL
        CHECK (quantity > 0),
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id)
);

-- =========================
-- DELIVERY & RECEIPTS
-- =========================

CREATE TABLE deliveries (
    id VARCHAR(40) PRIMARY KEY,
    order_id VARCHAR(40) NOT NULL,
    status VARCHAR(20) NOT NULL
        CHECK (status IN ('PENDING', 'IN_TRANSIT', 'DELIVERED', 'FAILED')),
    delivery_time TIMESTAMP,
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE
);

CREATE TABLE receipts (
    id VARCHAR(40) PRIMARY KEY,
    order_id VARCHAR(40) NOT NULL UNIQUE,
    received_at TIMESTAMP DEFAULT NOW(),
    quality_rating INT
        CHECK (quality_rating BETWEEN 1 AND 5),
    quality_note VARCHAR(255),
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE
);

-- =========================
-- AUDIT LOGS
-- =========================

CREATE TABLE audit_logs (
    id VARCHAR(40) PRIMARY KEY,
    entity_type VARCHAR(50) NOT NULL,
    entity_id VARCHAR(40) NOT NULL,
    user_id VARCHAR(40),
    action VARCHAR(30) NOT NULL
        CHECK (action IN ('CREATE', 'UPDATE', 'DELETE')),
    old_value TEXT,
    new_value TEXT,
    created_at TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY (user_id) REFERENCES users(id)
);

INSERT INTO roles (id, name) VALUES
('ROLE_FRANCHISE_STORE_STAFF', 'Franchise Store Staff'),
('ROLE_CENTRAL_KITCHEN_STAFF', 'Central Kitchen Staff'),
('ROLE_SUPPLY_COORDINATOR', 'Supply Coordinator'),
('ROLE_MANAGER', 'Manager'),
('ROLE_ADMIN', 'Admin');

INSERT INTO users (id, name, email, password_hash, status)
VALUES
-- Admin
('USER_20260128_ad01', 'Admin User',
 'admin@system.com',
 '$2a$11$C6UzMDM.H6dfI/f/IKcEeOXbGZL1Qp1z0p6sZ3Q5Z6sZ6Z6Z6Z6',
 'ACTIVE'),

-- Manager
('USER_20260128_mg02', 'Manager User',
 'manager@system.com',
 '$2a$11$C6UzMDM.H6dfI/f/IKcEeOXbGZL1Qp1z0p6sZ3Q5Z6sZ6Z6Z6Z6',
 'ACTIVE'),

-- Franchise Store Staff
('USER_20260128_fs03', 'Franchise Staff',
 'franchise@store.com',
 '$2a$11$C6UzMDM.H6dfI/f/IKcEeOXbGZL1Qp1z0p6sZ3Q5Z6sZ6Z6Z6Z6',
 'ACTIVE'),

-- Central Kitchen Staff
('USER_20260128_ks04', 'Kitchen Staff',
 'kitchen@central.com',
 '$2a$11$C6UzMDM.H6dfI/f/IKcEeOXbGZL1Qp1z0p6sZ3Q5Z6sZ6Z6Z6Z6',
 'ACTIVE'),

-- Supply Coordinator
('USER_20260128_sc05', 'Supply Coordinator',
 'supply@system.com',
 '$2a$11$C6UzMDM.H6dfI/f/IKcEeOXbGZL1Qp1z0p6sZ3Q5Z6sZ6Z6Z6Z6',
 'ACTIVE');



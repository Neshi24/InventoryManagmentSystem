
# InventoryManagmentSystem

A microservice application that uses kubernetes for orchistration.



## Installation

Requires* Docker and kubectl

Push docker images:

build images: docker-compose build

Push to cluster:

Get dashboard (optional): kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml

Get rabbitMq: kubectl apply -f "https://github.com/rabbitmq/cluster-operator/releases/latest/download/cluster-operator.yml"

Navigate to k8s folder: cd k8s/

run: kubectl apply -f .


    
## API Reference examples

#### RebuildDb

```http
  AuthController
Register
POST http://localhost:8085/Auth/Register

GetUserById
GET http://localhost:8085/Auth/{userId}

UpdateUser
PUT http://localhost:8085/Auth/update?userId={userId}

DeleteUserById
DELETE http://localhost:8085/Auth/delete?userId={userId}

Login
POST http://localhost:8085/Auth/login

AddTestUsers
POST http://localhost:8085/Auth/TestUsers

RebuildDB
POST http://localhost:8085/Auth/RebuildDB

ItemController
CreateItem
POST http://localhost:8085/Item/CreateItem

GetItemById
GET http://localhost:8085/Item/{id}

GetAllItems
GET http://localhost:8085/Item/GetAllItems

GetItemsByIds
GET http://localhost:8085/Item/GetItemsByIds?ids={ids}

UpdateItem
PUT http://localhost:8085/Item/{id}

DeleteItem
DELETE http://localhost:8085/Item/{id}

RebuildDB
POST http://localhost:8085/Item/RebuildDB

OrderController
CreateOrder
POST http://localhost:8085/Order/CreateOrder

GetOrderById
GET http://localhost:8085/Order/{id}

GetAllOrders
GET http://localhost:8085/Order/GetAllOrders

GetAllOrdersHistory
GET http://localhost:8085/Order/GetAllOrdersHistory

UpdateOrder
PUT http://localhost:8085/Order/{id}

DeleteOrder
DELETE http://localhost:8085/Order/{id}

RebuildDB
POST http://localhost:8085/Order/RebuildDB
```







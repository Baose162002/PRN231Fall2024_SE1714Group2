﻿using BusinessObject;
using BusinessObject.DTO.Response;
using BusinessObject.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrder();
        Task<List<ListOrderDTO>> GetAllOrdersByUserId(string userRole, int userId);
        Task<ListOrderDTO> GetOrderById(int id);

        Task Create(CreateOrderDTO order);
/*        Task CreateOrderByBatch(CreateOrderDTO order);
*/
        Task Update(UpdateOrderDTO updateOrderDTO, int id);
        Task Delete(int id);
        Task<Order> UpdateOrderStatus(int updatedOrder);
        Task<List<Order>> SearchOrders(OrderSearchDTO searchCriteria);
        Task<int> CreateOrder(CreateOrderFlowerDTO orderDTO);

        Task UpdateStatus(OrderUpdateStatusDTO order, int id);
    }
}

using Microsoft.EntityFrameworkCore;
using WorkOrderFlow.Web.Models;

namespace WorkOrderFlow.Web.Data;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Customers.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var customers = new[]
        {
            new Customer
            {
                FullName = "Ahmet Yılmaz",
                CompanyName = "Yılmaz Motor Servis",
                Phone = "0555 210 11 01",
                Email = "ahmet@yilmazmotor.example",
                Address = "Eskişehir / Tepebaşı",
                Notes = "Periyodik bakım ve filo servis müşterisi.",
                CreatedAt = now.AddDays(-45)
            },
            new Customer
            {
                FullName = "Selin Kaya",
                CompanyName = "Atlas Teknik",
                Phone = "0555 210 11 02",
                Email = "selin@atlasteknik.example",
                Address = "Eskişehir OSB",
                Notes = "Bakım sözleşmesi görüşülüyor.",
                CreatedAt = now.AddDays(-38)
            },
            new Customer
            {
                FullName = "Murat Demir",
                CompanyName = "Demir Lojistik",
                Phone = "0555 210 11 03",
                Email = "murat@demirlojistik.example",
                Address = "Eskişehir / Odunpazarı",
                Notes = "Hafif ticari araç bakım müşterisi.",
                CreatedAt = now.AddDays(-33)
            },
            new Customer
            {
                FullName = "Elif Aydın",
                CompanyName = "Aydın Endüstri",
                Phone = "0555 210 11 04",
                Email = "elif@aydinendustri.example",
                Address = "Bilecik",
                Notes = "Endüstriyel ekipman bakım müşterisi.",
                CreatedAt = now.AddDays(-28)
            },
            new Customer
            {
                FullName = "Burak Şahin",
                CompanyName = "Şahin Filo",
                Phone = "0555 210 11 05",
                Email = "burak@sahinfilo.example",
                Address = "Kütahya",
                Notes = "Filo bakım operasyonları.",
                CreatedAt = now.AddDays(-20)
            },
            new Customer
            {
                FullName = "Zeynep Arslan",
                CompanyName = "Arslan Enerji",
                Phone = "0555 210 11 06",
                Email = "zeynep@arslanenerji.example",
                Address = "Eskişehir / Batıkent",
                Notes = "Saha ekipmanı servis talepleri.",
                CreatedAt = now.AddDays(-14)
            }
        };

        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();

        var quotes = new[]
        {
            new Quote
            {
                CustomerId = customers[0].Id,
                Title = "RKS R250 bakım ve güvenlik kontrolü",
                LaborCost = 4_500,
                PartsCost = 3_600,
                Discount = 600,
                Status = QuoteStatus.Accepted,
                ValidUntil = now.AddDays(12),
                Notes = "Zincir seti, fren kontrolü ve genel bakım.",
                CreatedAt = now.AddDays(-18)
            },
            new Quote
            {
                CustomerId = customers[1].Id,
                Title = "Atölye ekipmanı periyodik bakım paketi",
                LaborCost = 10_500,
                PartsCost = 8_750,
                Discount = 750,
                Status = QuoteStatus.Sent,
                ValidUntil = now.AddDays(10),
                Notes = "Üç ekipman için bakım ve parça değişimi.",
                CreatedAt = now.AddDays(-12)
            },
            new Quote
            {
                CustomerId = customers[2].Id,
                Title = "Filo araçları ön kontrol teklifi",
                LaborCost = 6_500,
                PartsCost = 3_200,
                Discount = 500,
                Status = QuoteStatus.Draft,
                ValidUntil = now.AddDays(20),
                Notes = "Sekiz araç için ilk kontrol planı.",
                CreatedAt = now.AddDays(-8)
            },
            new Quote
            {
                CustomerId = customers[3].Id,
                Title = "Kompresör bakım ve sarf malzeme teklifi",
                LaborCost = 8_900,
                PartsCost = 6_800,
                Discount = 1_000,
                Status = QuoteStatus.Rejected,
                ValidUntil = now.AddDays(-2),
                Notes = "Müşteri alternatif teklif değerlendirdi.",
                CreatedAt = now.AddDays(-22)
            },
            new Quote
            {
                CustomerId = customers[4].Id,
                Title = "Filo fren sistemi kontrol paketi",
                LaborCost = 4_250,
                PartsCost = 4_000,
                Discount = 750,
                Status = QuoteStatus.Expired,
                ValidUntil = now.AddDays(-5),
                Notes = "Teklif geçerlilik süresi doldu.",
                CreatedAt = now.AddDays(-30)
            },
            new Quote
            {
                CustomerId = customers[5].Id,
                Title = "Saha jeneratörü ağır bakım hizmeti",
                LaborCost = 14_500,
                PartsCost = 11_750,
                Discount = 1_250,
                Status = QuoteStatus.Accepted,
                ValidUntil = now.AddDays(18),
                Notes = "Ağır bakım, akü ve filtre değişimi.",
                CreatedAt = now.AddDays(-16)
            },
            new Quote
            {
                CustomerId = customers[4].Id,
                Title = "Aylık filo bakım sözleşmesi",
                LaborCost = 7_800,
                PartsCost = 4_600,
                Discount = 600,
                Status = QuoteStatus.Sent,
                ValidUntil = now.AddDays(14),
                Notes = "Aylık planlı bakım ve raporlama.",
                CreatedAt = now.AddDays(-6)
            }
        };

        context.Quotes.AddRange(quotes);
        await context.SaveChangesAsync();

        var inventoryItems = new[]
        {
            new InventoryItem
            {
                Name = "Zincir Yağı",
                Sku = "CHAIN-OIL-001",
                Category = "Bakım",
                QuantityOnHand = 19,
                ReorderLevel = 5,
                UnitCost = 180,
                SalePrice = 250,
                SupplierName = "Anadolu Teknik",
                Location = "Raf A1",
                Notes = "Yüksek sıcaklık dayanımlı zincir yağı.",
                CreatedAt = now.AddDays(-60)
            },
            new InventoryItem
            {
                Name = "Fren Balatası",
                Sku = "BRAKE-PAD-001",
                Category = "Fren",
                QuantityOnHand = 7,
                ReorderLevel = 5,
                UnitCost = 720,
                SalePrice = 1_050,
                SupplierName = "Marmara Parça",
                Location = "Raf B2",
                Notes = "Ön fren balatası seti.",
                CreatedAt = now.AddDays(-55)
            },
            new InventoryItem
            {
                Name = "Hava Filtresi",
                Sku = "AIR-FILTER-001",
                Category = "Filtre",
                QuantityOnHand = 13,
                ReorderLevel = 4,
                UnitCost = 340,
                SalePrice = 520,
                SupplierName = "Filtre Merkezi",
                Location = "Raf C1",
                CreatedAt = now.AddDays(-50)
            },
            new InventoryItem
            {
                Name = "12V Akü",
                Sku = "BATTERY-12V-001",
                Category = "Elektrik",
                QuantityOnHand = 3,
                ReorderLevel = 2,
                UnitCost = 2_850,
                SalePrice = 3_750,
                SupplierName = "Güç Sistemleri",
                Location = "Depo D1",
                CreatedAt = now.AddDays(-48)
            },
            new InventoryItem
            {
                Name = "Soğutma Sıvısı",
                Sku = "COOLANT-001",
                Category = "Sıvı",
                QuantityOnHand = 11,
                ReorderLevel = 3,
                UnitCost = 260,
                SalePrice = 390,
                SupplierName = "Anadolu Teknik",
                Location = "Raf A3",
                CreatedAt = now.AddDays(-40)
            },
            new InventoryItem
            {
                Name = "Tahrik Kayışı",
                Sku = "DRIVE-BELT-001",
                Category = "Aktarma",
                QuantityOnHand = 4,
                ReorderLevel = 4,
                UnitCost = 1_100,
                SalePrice = 1_550,
                SupplierName = "Endüstri Parça",
                Location = "Raf E2",
                CreatedAt = now.AddDays(-35)
            }
        };

        context.InventoryItems.AddRange(inventoryItems);
        await context.SaveChangesAsync();

        foreach (var item in inventoryItems)
        {
            context.InventoryTransactions.Add(new InventoryTransaction
            {
                InventoryItemId = item.Id,
                Type = InventoryTransactionType.ManualAdjustment,
                QuantityChange = item.QuantityOnHand,
                QuantityAfter = item.QuantityOnHand,
                Notes = "Demo başlangıç stok kaydı",
                CreatedAt = item.CreatedAt.AddHours(2)
            });
        }

        var workOrders = new[]
        {
            new WorkOrder
            {
                CustomerId = customers[0].Id,
                QuoteId = quotes[0].Id,
                Title = "RKS R250 zincir ve fren bakımı",
                Description = "Zincir temizliği, yağlama ve fren güvenlik kontrolü.",
                Status = WorkOrderStatus.InProgress,
                Priority = WorkOrderPriority.High,
                CreatedAt = now.AddDays(-7),
                DueDate = now.AddDays(2)
            },
            new WorkOrder
            {
                CustomerId = customers[1].Id,
                Title = "Atölye kompresörü bakım işlemi",
                Description = "Basınç kontrolü ve aşınan parçaların değişimi.",
                Status = WorkOrderStatus.WaitingParts,
                Priority = WorkOrderPriority.Urgent,
                CreatedAt = now.AddDays(-10),
                DueDate = now.AddDays(-1)
            },
            new WorkOrder
            {
                CustomerId = customers[2].Id,
                Title = "Filo aracı periyodik bakım",
                Description = "Filtre, sıvı ve genel mekanik kontrol.",
                Status = WorkOrderStatus.Completed,
                Priority = WorkOrderPriority.Medium,
                CreatedAt = now.AddDays(-14),
                DueDate = now.AddDays(-6),
                CompletedAt = now.AddDays(-5),
                ResolutionNote = "Bakım tamamlandı ve test sürüşü yapıldı."
            },
            new WorkOrder
            {
                CustomerId = customers[5].Id,
                QuoteId = quotes[5].Id,
                Title = "Saha jeneratörü ağır bakım",
                Description = "Akü, filtre ve soğutma sistemi bakımı.",
                Status = WorkOrderStatus.Delivered,
                Priority = WorkOrderPriority.High,
                CreatedAt = now.AddDays(-20),
                DueDate = now.AddDays(-11),
                CompletedAt = now.AddDays(-10),
                ResolutionNote = "Bakım tamamlandı, saha ekibine teslim edildi."
            },
            new WorkOrder
            {
                CustomerId = customers[3].Id,
                Title = "Kompresör titreşim analizi",
                Description = "Titreşim kaynağı ve yatak kontrolü.",
                Status = WorkOrderStatus.New,
                Priority = WorkOrderPriority.Medium,
                CreatedAt = now.AddDays(-2),
                DueDate = now.AddDays(5)
            },
            new WorkOrder
            {
                CustomerId = customers[4].Id,
                Title = "Filo aracı kayış değişimi",
                Description = "Aşınan tahrik kayışının değişimi.",
                Status = WorkOrderStatus.Approved,
                Priority = WorkOrderPriority.Medium,
                CreatedAt = now.AddDays(-4),
                DueDate = now.AddDays(3)
            },
            new WorkOrder
            {
                CustomerId = customers[3].Id,
                Title = "Eski bakım talebi",
                Description = "Müşteri talebi üzerine iptal edilen iş emri.",
                Status = WorkOrderStatus.Cancelled,
                Priority = WorkOrderPriority.Low,
                CreatedAt = now.AddDays(-18),
                DueDate = now.AddDays(-12),
                ResolutionNote = "Müşteri plan değişikliği nedeniyle iptal etti."
            },
            new WorkOrder
            {
                CustomerId = customers[4].Id,
                Title = "Fren sistemi arıza kontrolü",
                Description = "Fren sesi ve pedal hissi kontrolü.",
                Status = WorkOrderStatus.InProgress,
                Priority = WorkOrderPriority.High,
                CreatedAt = now.AddDays(-3),
                DueDate = now.AddDays(1)
            }
        };

        context.WorkOrders.AddRange(workOrders);
        await context.SaveChangesAsync();

        void AddHistory(
            WorkOrder workOrder,
            WorkOrderStatus? from,
            WorkOrderStatus to,
            string notes,
            DateTime createdAt)
        {
            context.WorkOrderStatusHistories.Add(new WorkOrderStatusHistory
            {
                WorkOrderId = workOrder.Id,
                FromStatus = from,
                ToStatus = to,
                Notes = notes,
                CreatedAt = createdAt
            });
        }

        AddHistory(workOrders[0], WorkOrderStatus.New, WorkOrderStatus.Approved, "İş emri onaylandı", now.AddDays(-7).AddHours(2));
        AddHistory(workOrders[0], WorkOrderStatus.Approved, WorkOrderStatus.InProgress, "Bakım çalışması başlatıldı", now.AddDays(-6));

        AddHistory(workOrders[1], WorkOrderStatus.New, WorkOrderStatus.Approved, "Teknik plan onaylandı", now.AddDays(-10).AddHours(3));
        AddHistory(workOrders[1], WorkOrderStatus.Approved, WorkOrderStatus.InProgress, "Söküm ve kontrol başladı", now.AddDays(-9));
        AddHistory(workOrders[1], WorkOrderStatus.InProgress, WorkOrderStatus.WaitingParts, "Fren balatası tedariki bekleniyor", now.AddDays(-2));

        AddHistory(workOrders[2], WorkOrderStatus.New, WorkOrderStatus.InProgress, "Araç servise alındı", now.AddDays(-13));
        AddHistory(workOrders[2], WorkOrderStatus.InProgress, WorkOrderStatus.Completed, "Bakım ve test tamamlandı", now.AddDays(-5));

        AddHistory(workOrders[3], WorkOrderStatus.New, WorkOrderStatus.InProgress, "Jeneratör bakımına başlandı", now.AddDays(-19));
        AddHistory(workOrders[3], WorkOrderStatus.InProgress, WorkOrderStatus.Completed, "Ağır bakım tamamlandı", now.AddDays(-10));
        AddHistory(workOrders[3], WorkOrderStatus.Completed, WorkOrderStatus.Delivered, "Saha ekibine teslim edildi", now.AddDays(-9));

        AddHistory(workOrders[5], WorkOrderStatus.New, WorkOrderStatus.Approved, "Kayış değişimi onaylandı", now.AddDays(-3));
        AddHistory(workOrders[6], WorkOrderStatus.New, WorkOrderStatus.Cancelled, "Müşteri tarafından iptal edildi", now.AddDays(-17));
        AddHistory(workOrders[7], WorkOrderStatus.New, WorkOrderStatus.InProgress, "Fren kontrolü başlatıldı", now.AddDays(-2));

        void AddMaterialUsage(
            WorkOrder workOrder,
            InventoryItem item,
            int quantity,
            DateTime usedAt,
            string notes)
        {
            item.QuantityOnHand -= quantity;

            context.WorkOrderMaterials.Add(new WorkOrderMaterial
            {
                WorkOrderId = workOrder.Id,
                InventoryItemId = item.Id,
                QuantityUsed = quantity,
                UnitPrice = item.SalePrice,
                UsedAt = usedAt,
                Notes = notes
            });

            context.InventoryTransactions.Add(new InventoryTransaction
            {
                InventoryItemId = item.Id,
                WorkOrderId = workOrder.Id,
                Type = InventoryTransactionType.WorkOrderUsage,
                QuantityChange = -quantity,
                QuantityAfter = item.QuantityOnHand,
                Notes = $"Malzeme iş emrinde kullanıldı #{workOrder.Id}",
                CreatedAt = usedAt
            });
        }

        AddMaterialUsage(workOrders[0], inventoryItems[0], 1, now.AddDays(-5), "Zincir bakımında kullanıldı.");

        inventoryItems[0].QuantityOnHand += 1;
        context.InventoryTransactions.Add(new InventoryTransaction
        {
            InventoryItemId = inventoryItems[0].Id,
            WorkOrderId = workOrders[0].Id,
            Type = InventoryTransactionType.WorkOrderUsageReversal,
            QuantityChange = 1,
            QuantityAfter = inventoryItems[0].QuantityOnHand,
            Notes = "Yanlış malzeme işlemi geri alındı",
            CreatedAt = now.AddDays(-5).AddMinutes(10)
        });

        inventoryItems[0].QuantityOnHand -= 1;
        context.InventoryTransactions.Add(new InventoryTransaction
        {
            InventoryItemId = inventoryItems[0].Id,
            WorkOrderId = workOrders[0].Id,
            Type = InventoryTransactionType.WorkOrderUsageCorrection,
            QuantityChange = -1,
            QuantityAfter = inventoryItems[0].QuantityOnHand,
            Notes = "Doğru stok hareketi yeniden uygulandı",
            CreatedAt = now.AddDays(-5).AddMinutes(20)
        });

        AddMaterialUsage(workOrders[1], inventoryItems[1], 2, now.AddDays(-8), "Kompresör fren mekanizması için ayrıldı.");
        AddMaterialUsage(workOrders[2], inventoryItems[2], 1, now.AddDays(-7), "Periyodik bakımda filtre değiştirildi.");
        AddMaterialUsage(workOrders[2], inventoryItems[4], 2, now.AddDays(-7).AddMinutes(15), "Soğutma sıvısı yenilendi.");
        AddMaterialUsage(workOrders[3], inventoryItems[3], 1, now.AddDays(-12), "Jeneratör aküsü değiştirildi.");
        AddMaterialUsage(workOrders[5], inventoryItems[5], 1, now.AddDays(-2), "Kayış iş emri için rezerve edildi.");
        AddMaterialUsage(workOrders[7], inventoryItems[1], 1, now.AddDays(-1), "Fren kontrolünde test parçası kullanıldı.");

        await context.SaveChangesAsync();
    }
}

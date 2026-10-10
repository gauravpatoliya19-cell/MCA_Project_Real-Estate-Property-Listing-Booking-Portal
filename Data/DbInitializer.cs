using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Services.Security;

namespace RealEstatePortal.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Auto-upgrade schema if table existed from earlier version
            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE ApplicationUsers ADD COLUMN PasswordHash TEXT DEFAULT '';");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE ApplicationUsers ADD COLUMN PasswordSalt TEXT DEFAULT '';");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE ApplicationUsers ADD COLUMN IsActive INTEGER DEFAULT 1;");
            }
            catch { }

            // Check if old US data exists; if so, clear to upgrade to Gujarat data
            try
            {
                if (context.Properties.Any(p => p.City == "Miami" || p.State == "FL" || p.Address.Contains("Ocean Boulevard")))
                {
                    context.RentalContracts.RemoveRange(context.RentalContracts);
                    context.TokenPayments.RemoveRange(context.TokenPayments);
                    context.SiteVisitBookings.RemoveRange(context.SiteVisitBookings);
                    context.Inquiries.RemoveRange(context.Inquiries);
                    context.PropertyFeatures.RemoveRange(context.PropertyFeatures);
                    context.PropertyImages.RemoveRange(context.PropertyImages);
                    context.Properties.RemoveRange(context.Properties);

                    var oldUsers = context.ApplicationUsers.Where(u => 
                        u.Email == "marcus.sterling@realestateportal.com" || 
                        u.Email == "david.chen@nycapital.com" ||
                        u.Email == "elena.vance@realestateportal.com" ||
                        u.Email == "sarah.jenkins@realestateportal.com").ToList();
                    context.ApplicationUsers.RemoveRange(oldUsers);
                    context.SaveChanges();
                }
            }
            catch { }

            // 1. GUARANTEE DEFAULT ADMIN USERS ALWAYS EXIST: Chavda Meet, Gaurav Patoliya, Jatapara Ronak
            try
            {
                var (easyAdminHash, easyAdminSalt) = PasswordHasher.HashPassword("123456");

                // Admin 1: Chavda Meet (Master Admin)
                var meetAdmin = context.ApplicationUsers.FirstOrDefault(u => 
                    u.Email.ToLower() == "meet@admin.com" || 
                    u.Email.ToLower() == "chavda.meet@realestateportal.com" ||
                    u.Email.ToLower() == "admin@realestateportal.com" ||
                    u.FullName.ToLower().Contains("chavda meet") ||
                    u.FullName.ToLower().Contains("meet"));

                if (meetAdmin == null)
                {
                    context.ApplicationUsers.Add(new ApplicationUser
                    {
                        FullName = "Chavda Meet",
                        Email = "meet@admin.com",
                        PhoneNumber = "+91 98250 11001",
                        Role = "Admin",
                        PasswordHash = easyAdminHash,
                        PasswordSalt = easyAdminSalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    meetAdmin.FullName = "Chavda Meet";
                    meetAdmin.Role = "Admin";
                    meetAdmin.IsActive = true;
                    meetAdmin.PasswordHash = easyAdminHash;
                    meetAdmin.PasswordSalt = easyAdminSalt;
                }

                // Also ensure admin@realestateportal.com exists as an alias for Chavda Meet
                if (!context.ApplicationUsers.Any(u => u.Email == "admin@realestateportal.com"))
                {
                    context.ApplicationUsers.Add(new ApplicationUser
                    {
                        FullName = "Chavda Meet (Admin)",
                        Email = "admin@realestateportal.com",
                        PhoneNumber = "+91 98250 11001",
                        Role = "Admin",
                        PasswordHash = easyAdminHash,
                        PasswordSalt = easyAdminSalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                // Admin 2: Gaurav Patoliya
                var gauravAdmin = context.ApplicationUsers.FirstOrDefault(u => 
                    u.Email.ToLower() == "gaurav@admin.com" || 
                    u.Email.ToLower() == "gaurav.patoliya@realestateportal.com" ||
                    u.FullName.ToLower().Contains("gaurav patoliya") ||
                    u.FullName.ToLower().Contains("gaurav"));

                if (gauravAdmin == null)
                {
                    context.ApplicationUsers.Add(new ApplicationUser
                    {
                        FullName = "Gaurav Patoliya",
                        Email = "gaurav@admin.com",
                        PhoneNumber = "+91 98250 11002",
                        Role = "Admin",
                        PasswordHash = easyAdminHash,
                        PasswordSalt = easyAdminSalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    gauravAdmin.FullName = "Gaurav Patoliya";
                    gauravAdmin.Role = "Admin";
                    gauravAdmin.IsActive = true;
                    gauravAdmin.PasswordHash = easyAdminHash;
                    gauravAdmin.PasswordSalt = easyAdminSalt;
                }

                // Admin 3: Jatapara Ronak
                var ronakAdmin = context.ApplicationUsers.FirstOrDefault(u => 
                    u.Email.ToLower() == "ronak@admin.com" || 
                    u.Email.ToLower() == "jatapara.ronak@realestateportal.com" ||
                    u.FullName.ToLower().Contains("jatapara ronak") ||
                    u.FullName.ToLower().Contains("ronak"));

                if (ronakAdmin == null)
                {
                    context.ApplicationUsers.Add(new ApplicationUser
                    {
                        FullName = "Jatapara Ronak",
                        Email = "ronak@admin.com",
                        PhoneNumber = "+91 98250 11003",
                        Role = "Admin",
                        PasswordHash = easyAdminHash,
                        PasswordSalt = easyAdminSalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    ronakAdmin.FullName = "Jatapara Ronak";
                    ronakAdmin.Role = "Admin";
                    ronakAdmin.IsActive = true;
                    ronakAdmin.PasswordHash = easyAdminHash;
                    ronakAdmin.PasswordSalt = easyAdminSalt;
                }

                context.SaveChanges();

                // Sample Gujarat Agents
                if (!context.ApplicationUsers.Any(u => u.Email == "chirag.patel@realestateportal.com"))
                {
                    var (agentHash, agentSalt) = PasswordHasher.HashPassword("Agent@123");
                    context.ApplicationUsers.Add(new ApplicationUser
                    {
                        FullName = "Chirag Patel",
                        Email = "chirag.patel@realestateportal.com",
                        PhoneNumber = "+91 98250 12345",
                        Role = "Agent",
                        PasswordHash = agentHash,
                        PasswordSalt = agentSalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                    context.SaveChanges();
                }

                if (!context.ApplicationUsers.Any(u => u.Email == "priya.shah@realestateportal.com"))
                {
                    var (agentHash, agentSalt) = PasswordHasher.HashPassword("Agent@123");
                    context.ApplicationUsers.Add(new ApplicationUser
                    {
                        FullName = "Priya Shah",
                        Email = "priya.shah@realestateportal.com",
                        PhoneNumber = "+91 98791 23456",
                        Role = "Agent",
                        PasswordHash = agentHash,
                        PasswordSalt = agentSalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                    context.SaveChanges();
                }

                if (!context.ApplicationUsers.Any(u => u.Email == "bhavin.mehta@realestateportal.com"))
                {
                    var (agentHash, agentSalt) = PasswordHasher.HashPassword("Agent@123");
                    context.ApplicationUsers.Add(new ApplicationUser
                    {
                        FullName = "Bhavin Mehta",
                        Email = "bhavin.mehta@realestateportal.com",
                        PhoneNumber = "+91 98240 56789",
                        Role = "Agent",
                        PasswordHash = agentHash,
                        PasswordSalt = agentSalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                    context.SaveChanges();
                }

                // Sample Gujarat Demo Customer
                if (!context.ApplicationUsers.Any(u => u.Email == "rajesh.patel@gmail.com"))
                {
                    var (userHash, userSalt) = PasswordHasher.HashPassword("User@123");
                    context.ApplicationUsers.Add(new ApplicationUser
                    {
                        FullName = "Rajesh Patel",
                        Email = "rajesh.patel@gmail.com",
                        PhoneNumber = "+91 98251 77340",
                        Role = "Customer",
                        PasswordHash = userHash,
                        PasswordSalt = userSalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                    context.SaveChanges();
                }
            }
            catch { }

            if (context.Properties.Any())
            {
                return; // Properties already seeded
            }

            var properties = new List<Property>
            {
                new Property
                {
                    Title = "The Grand Riviera Luxury Penthouse",
                    Description = "Experience pinnacle living in this breathtaking 4-bedroom penthouse overlooking the Sabarmati Riverfront and SG Highway. Features 360-degree panoramic skyline views, private infinity rooftop deck, Italian marble finishes, soundproof double-glazed glass, smart home automation, and private elevator access.",
                    PropertyType = PropertyType.Penthouse,
                    ListingType = ListingType.Sale,
                    Status = PropertyStatus.Available,
                    Price = 28500000m,
                    TokenAmount = 500000m,
                    Address = "742 Riverfront Boulevard, Penthouse 18B, Bodakdev",
                    City = "Ahmedabad",
                    State = "Gujarat",
                    ZipCode = "380054",
                    Bedrooms = 4,
                    Bathrooms = 5,
                    AreaSqFt = 4850,
                    YearBuilt = 2023,
                    IsFeatured = true,
                    AgentName = "Chirag Patel",
                    AgentEmail = "chirag.patel@realestateportal.com",
                    AgentPhone = "+91 98250 12345",
                    CommissionRate = 2.0m,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Property
                {
                    Title = "Silicon Greens Modern Architectural Villa",
                    Description = "Ultra-modern architectural masterpiece nestled in exclusive gated community near Sindhu Bhavan Road. Features minimalist cantilever design, private landscaped garden, heated swimming pool, rooftop solar energy system, and custom modular Italian kitchen.",
                    PropertyType = PropertyType.Villa,
                    ListingType = ListingType.Sale,
                    Status = PropertyStatus.Booked,
                    Price = 17500000m,
                    TokenAmount = 300000m,
                    Address = "1204 Palm Crest Greens, Sindhu Bhavan Road",
                    City = "Ahmedabad",
                    State = "Gujarat",
                    ZipCode = "380059",
                    Bedrooms = 5,
                    Bathrooms = 6,
                    AreaSqFt = 5200,
                    YearBuilt = 2022,
                    IsFeatured = true,
                    AgentName = "Priya Shah",
                    AgentEmail = "priya.shah@realestateportal.com",
                    AgentPhone = "+91 98791 23456",
                    CommissionRate = 2.5m,
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Property
                {
                    Title = "Dumas Luxury Executive High-Rise Loft",
                    Description = "Sun-drenched authentic luxury loft in prime Vesu, Surat. Soaring 14ft high ceilings, expansive French windows, modular chef's kitchen with Bosch appliances, spa bathroom with soaking jacuzzi tub, and bespoke walk-in wardrobe.",
                    PropertyType = PropertyType.Apartment,
                    ListingType = ListingType.Rent,
                    Status = PropertyStatus.Rented,
                    Price = 45000m,
                    TokenAmount = 15000m,
                    Address = "184 Vesu Canal Corridor, Apt 8A",
                    City = "Surat",
                    State = "Gujarat",
                    ZipCode = "395007",
                    Bedrooms = 3,
                    Bathrooms = 3,
                    AreaSqFt = 2100,
                    YearBuilt = 2021,
                    IsFeatured = true,
                    AgentName = "Chirag Patel",
                    AgentEmail = "chirag.patel@realestateportal.com",
                    AgentPhone = "+91 98250 12345",
                    CommissionRate = 8.33m,
                    CreatedAt = DateTime.UtcNow.AddMonths(-5)
                },
                new Property
                {
                    Title = "Alkapuri Contemporary Heritage Estate with Pool",
                    Description = "Gated private sanctuary in prestigious Alkapuri featuring seamless indoor-outdoor living, private home theatre, gym, guest annexe, lush mature garden landscaping, and private swimming pool.",
                    PropertyType = PropertyType.Villa,
                    ListingType = ListingType.Sale,
                    Status = PropertyStatus.Sold,
                    Price = 35000000m,
                    TokenAmount = 500000m,
                    Address = "42 RC Dutt Road, Alkapuri",
                    City = "Vadodara",
                    State = "Gujarat",
                    ZipCode = "390007",
                    Bedrooms = 6,
                    Bathrooms = 7,
                    AreaSqFt = 6800,
                    YearBuilt = 2021,
                    IsFeatured = false,
                    AgentName = "Bhavin Mehta",
                    AgentEmail = "bhavin.mehta@realestateportal.com",
                    AgentPhone = "+91 98240 56789",
                    CommissionRate = 2.0m,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new Property
                {
                    Title = "GIFT City FinTech Skyline Luxury Residence",
                    Description = "Corner unit high-rise residence overlooking the global financial district of GIFT City. Includes 24/7 concierge, private multi-level parking with EV charging stations, district cooling, smart utility metering, and panoramic balconies.",
                    PropertyType = PropertyType.Apartment,
                    ListingType = ListingType.Rent,
                    Status = PropertyStatus.Available,
                    Price = 55000m,
                    TokenAmount = 15000m,
                    Address = "Tower B, Suite 1902, GIFT City International Zone",
                    City = "Gandhinagar",
                    State = "Gujarat",
                    ZipCode = "382355",
                    Bedrooms = 3,
                    Bathrooms = 3,
                    AreaSqFt = 1950,
                    YearBuilt = 2023,
                    IsFeatured = true,
                    AgentName = "Priya Shah",
                    AgentEmail = "priya.shah@realestateportal.com",
                    AgentPhone = "+91 98791 23456",
                    CommissionRate = 8.33m,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                },
                new Property
                {
                    Title = "Kalawad Road Royal Heritage Townhouse",
                    Description = "Impeccably restored 3-story luxury townhouse on wide tree-lined Kalawad Road. Combines royal Kathiyawadi architectural elements with modern amenities: teakwood craftsmanship, private rooftop garden, and private elevator.",
                    PropertyType = PropertyType.Townhouse,
                    ListingType = ListingType.Sale,
                    Status = PropertyStatus.Available,
                    Price = 19500000m,
                    TokenAmount = 250000m,
                    Address = "38 Imperial Heights Avenue, Kalawad Road",
                    City = "Rajkot",
                    State = "Gujarat",
                    ZipCode = "360005",
                    Bedrooms = 4,
                    Bathrooms = 4,
                    AreaSqFt = 3800,
                    YearBuilt = 2020,
                    IsFeatured = false,
                    AgentName = "Chirag Patel",
                    AgentEmail = "chirag.patel@realestateportal.com",
                    AgentPhone = "+91 98250 12345",
                    CommissionRate = 2.5m,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                },
                new Property
                {
                    Title = "SG Highway Tech & Commercial Innovation Hub",
                    Description = "Modern grade-A commercial office space ideal for high-growth tech enterprises, finance firms, or corporate headquarters. High-speed fiber connectivity, flexible open floorplate, executive boardroom suites, and cafeteria lounge.",
                    PropertyType = PropertyType.Commercial,
                    ListingType = ListingType.Rent,
                    Status = PropertyStatus.Available,
                    Price = 125000m,
                    TokenAmount = 35000m,
                    Address = "820 Titanium Square, SG Highway, Prahlad Nagar",
                    City = "Ahmedabad",
                    State = "Gujarat",
                    ZipCode = "380015",
                    Bedrooms = 0,
                    Bathrooms = 4,
                    AreaSqFt = 5800,
                    YearBuilt = 2021,
                    IsFeatured = false,
                    AgentName = "Bhavin Mehta",
                    AgentEmail = "bhavin.mehta@realestateportal.com",
                    AgentPhone = "+91 98240 56789",
                    CommissionRate = 5.0m,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                },
                new Property
                {
                    Title = "Piplod Waterfront Coastal Executive Studio",
                    Description = "Chic turnkey studio apartment overlooking serene Tapi riverfront walkway in Piplod. Features modular fold-away bed, quartz breakfast bar, riverside promenade access, clubhouse gym, and swimming pool.",
                    PropertyType = PropertyType.Studio,
                    ListingType = ListingType.Rent,
                    Status = PropertyStatus.Booked,
                    Price = 22000m,
                    TokenAmount = 8000m,
                    Address = "410 River Breeze Residency, Piplod Main Road",
                    City = "Surat",
                    State = "Gujarat",
                    ZipCode = "395007",
                    Bedrooms = 1,
                    Bathrooms = 1,
                    AreaSqFt = 650,
                    YearBuilt = 2022,
                    IsFeatured = false,
                    AgentName = "Priya Shah",
                    AgentEmail = "priya.shah@realestateportal.com",
                    AgentPhone = "+91 98791 23456",
                    CommissionRate = 8.33m,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                }
            };

            context.Properties.AddRange(properties);
            context.SaveChanges();

            // Images
            var images = new List<PropertyImage>
            {
                new PropertyImage { PropertyId = properties[0].Id, ImageUrl = "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=1200&q=80", IsPrimary = true, Caption = "Terrace Skyline View" },
                new PropertyImage { PropertyId = properties[0].Id, ImageUrl = "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?auto=format&fit=crop&w=1200&q=80", IsPrimary = false, Caption = "Living Hall" },
                new PropertyImage { PropertyId = properties[0].Id, ImageUrl = "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=1200&q=80", IsPrimary = false, Caption = "Master Suite" },

                new PropertyImage { PropertyId = properties[1].Id, ImageUrl = "https://images.unsplash.com/photo-1613977257363-707ba9348227?auto=format&fit=crop&w=1200&q=80", IsPrimary = true, Caption = "Exterior Front View" },
                new PropertyImage { PropertyId = properties[1].Id, ImageUrl = "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?auto=format&fit=crop&w=1200&q=80", IsPrimary = false, Caption = "Private Garden & Pool Deck" },

                new PropertyImage { PropertyId = properties[2].Id, ImageUrl = "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?auto=format&fit=crop&w=1200&q=80", IsPrimary = true, Caption = "Main Loft Space" },
                new PropertyImage { PropertyId = properties[2].Id, ImageUrl = "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?auto=format&fit=crop&w=1200&q=80", IsPrimary = false, Caption = "Modular Kitchen & Dining" },

                new PropertyImage { PropertyId = properties[3].Id, ImageUrl = "https://images.unsplash.com/photo-1580587771525-78b9dba3b914?auto=format&fit=crop&w=1200&q=80", IsPrimary = true, Caption = "Alkapuri Estate Facade" },
                new PropertyImage { PropertyId = properties[3].Id, ImageUrl = "https://images.unsplash.com/photo-1513694203232-719a280e022f?auto=format&fit=crop&w=1200&q=80", IsPrimary = false, Caption = "Grand Living Lounge" },

                new PropertyImage { PropertyId = properties[4].Id, ImageUrl = "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?auto=format&fit=crop&w=1200&q=80", IsPrimary = true, Caption = "GIFT City Skyline View" },
                new PropertyImage { PropertyId = properties[4].Id, ImageUrl = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=1200&q=80", IsPrimary = false, Caption = "Premium Bedroom Suite" },

                new PropertyImage { PropertyId = properties[5].Id, ImageUrl = "https://images.unsplash.com/photo-1576941089067-2de3c901e126?auto=format&fit=crop&w=1200&q=80", IsPrimary = true, Caption = "Heritage Townhouse Entry" },

                new PropertyImage { PropertyId = properties[6].Id, ImageUrl = "https://images.unsplash.com/photo-1497366216548-37526070297c?auto=format&fit=crop&w=1200&q=80", IsPrimary = true, Caption = "Commercial Floorplate" },

                new PropertyImage { PropertyId = properties[7].Id, ImageUrl = "https://images.unsplash.com/photo-1536376072261-38c75010e6c9?auto=format&fit=crop&w=1200&q=80", IsPrimary = true, Caption = "Riverfront Studio Interior" }
            };
            context.PropertyImages.AddRange(images);

            // Features
            var features = new List<PropertyFeature>();
            foreach (var prop in properties)
            {
                features.Add(new PropertyFeature { PropertyId = prop.Id, FeatureName = "24/7 Security & CCTV", IconClass = "bi-shield-check" });
                features.Add(new PropertyFeature { PropertyId = prop.Id, FeatureName = "Dedicated Covered Parking", IconClass = "bi-p-circle" });
                features.Add(new PropertyFeature { PropertyId = prop.Id, FeatureName = "High-Speed Fiber Internet", IconClass = "bi-wifi" });
                features.Add(new PropertyFeature { PropertyId = prop.Id, FeatureName = "24/7 Water Supply & Backup", IconClass = "bi-droplet-half" });

                if (prop.PropertyType == PropertyType.Penthouse || prop.PropertyType == PropertyType.Villa)
                {
                    features.Add(new PropertyFeature { PropertyId = prop.Id, FeatureName = "Private Swimming Pool", IconClass = "bi-water" });
                    features.Add(new PropertyFeature { PropertyId = prop.Id, FeatureName = "Rooftop Terrace Garden", IconClass = "bi-sun" });
                    features.Add(new PropertyFeature { PropertyId = prop.Id, FeatureName = "Solar Power System", IconClass = "bi-lightning-charge" });
                }
            }
            context.PropertyFeatures.AddRange(features);
            context.SaveChanges();

            // Inquiries
            var inquiries = new List<Inquiry>
            {
                new Inquiry
                {
                    PropertyId = properties[0].Id,
                    CustomerName = "Rajesh Patel",
                    CustomerEmail = "rajesh.patel@gmail.com",
                    CustomerPhone = "+91 98251 77340",
                    Message = "Interested in scheduling a VIP walkthrough this Saturday afternoon at Bodakdev. Please confirm available slots.",
                    InquiryDate = DateTime.UtcNow.AddDays(-28),
                    Status = InquiryStatus.ConvertedToSiteVisit
                },
                new Inquiry
                {
                    PropertyId = properties[1].Id,
                    CustomerName = "Hiren Desai",
                    CustomerEmail = "hiren.desai@gmail.com",
                    CustomerPhone = "+91 98252 89123",
                    Message = "Looking to inspect the private garden and solar backup system for the villa on Sindhu Bhavan Road.",
                    InquiryDate = DateTime.UtcNow.AddDays(-45),
                    Status = InquiryStatus.ConvertedToBooking
                },
                new Inquiry
                {
                    PropertyId = properties[2].Id,
                    CustomerName = "Rajesh Patel",
                    CustomerEmail = "rajesh.patel@gmail.com",
                    CustomerPhone = "+91 98251 77340",
                    Message = "Inquiring about a 24-month lease term for the Vesu luxury loft starting next month.",
                    InquiryDate = DateTime.UtcNow.AddDays(-60),
                    Status = InquiryStatus.ConvertedToBooking
                },
                new Inquiry
                {
                    PropertyId = properties[3].Id,
                    CustomerName = "Anjali Trivedi",
                    CustomerEmail = "anjali.trivedi@techgujarat.in",
                    CustomerPhone = "+91 98253 44198",
                    Message = "Representing a high-net-worth family looking to acquire the Alkapuri estate. Please share title deed verification pack.",
                    InquiryDate = DateTime.UtcNow.AddDays(-90),
                    Status = InquiryStatus.ConvertedToBooking
                }
            };
            context.Inquiries.AddRange(inquiries);

            // Site Visits
            var bookings = new List<SiteVisitBooking>
            {
                new SiteVisitBooking
                {
                    PropertyId = properties[0].Id,
                    CustomerName = "Rajesh Patel",
                    CustomerEmail = "rajesh.patel@gmail.com",
                    CustomerPhone = "+91 98251 77340",
                    VisitDate = DateTime.UtcNow.AddDays(2),
                    TimeSlot = "02:00 PM - 03:30 PM",
                    Notes = "Bringing interior designer and architect to review space.",
                    Status = BookingStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow.AddDays(-25)
                },
                new SiteVisitBooking
                {
                    PropertyId = properties[1].Id,
                    CustomerName = "Hiren Desai",
                    CustomerEmail = "hiren.desai@gmail.com",
                    CustomerPhone = "+91 98252 89123",
                    VisitDate = DateTime.UtcNow.AddDays(-40),
                    TimeSlot = "10:00 AM - 11:30 AM",
                    Notes = "Inspected property and loved the private garden & pool deck.",
                    Status = BookingStatus.Completed,
                    CreatedAt = DateTime.UtcNow.AddDays(-44)
                },
                new SiteVisitBooking
                {
                    PropertyId = properties[2].Id,
                    CustomerName = "Rajesh Patel",
                    CustomerEmail = "rajesh.patel@gmail.com",
                    CustomerPhone = "+91 98251 77340",
                    VisitDate = DateTime.UtcNow.AddDays(-55),
                    TimeSlot = "04:00 PM - 05:00 PM",
                    Notes = "Evening walkthrough to inspect ventilation and natural lighting.",
                    Status = BookingStatus.Completed,
                    CreatedAt = DateTime.UtcNow.AddDays(-58)
                }
            };
            context.SiteVisitBookings.AddRange(bookings);

            // Token Payments
            var payments = new List<TokenPayment>
            {
                new TokenPayment
                {
                    PropertyId = properties[1].Id,
                    CustomerName = "Hiren Desai",
                    CustomerEmail = "hiren.desai@gmail.com",
                    CustomerPhone = "+91 98252 89123",
                    Amount = 300000m,
                    TransactionId = "TXN-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),
                    ReceiptNumber = "REC-2026-0811",
                    PaymentMethod = PaymentMethod.NetBanking,
                    Status = PaymentStatus.Success,
                    PaymentDate = DateTime.UtcNow.AddDays(-38),
                    CardLast4 = "8812",
                    GatewayReference = "HDFC-PG-" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                    Notes = "Token earnest reservation deposit for Sindhu Bhavan Villa."
                },
                new TokenPayment
                {
                    PropertyId = properties[2].Id,
                    CustomerName = "Rajesh Patel",
                    CustomerEmail = "rajesh.patel@gmail.com",
                    CustomerPhone = "+91 98251 77340",
                    Amount = 15000m,
                    TransactionId = "TXN-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),
                    ReceiptNumber = "REC-2026-0749",
                    PaymentMethod = PaymentMethod.CreditCard,
                    Status = PaymentStatus.Success,
                    PaymentDate = DateTime.UtcNow.AddDays(-52),
                    CardLast4 = "4242",
                    GatewayReference = "ICICI-PG-" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                    Notes = "Holding reservation deposit for Vesu Luxury Loft."
                }
            };
            context.TokenPayments.AddRange(payments);

            // Rental Contracts
            var contracts = new List<RentalContract>
            {
                new RentalContract
                {
                    PropertyId = properties[2].Id,
                    ContractNumber = "LSE-2026-SUR-0092",
                    TenantName = "Rajesh Patel",
                    TenantEmail = "rajesh.patel@gmail.com",
                    TenantPhone = "+91 98251 77340",
                    TenantIdProof = "AADHAAR: 9821-4710-3342",
                    LandlordName = "Real Estate Property Gujarat Pvt Ltd",
                    LandlordEmail = "leases@realestateproperty.in",
                    StartDate = DateTime.UtcNow.AddDays(-50),
                    EndDate = DateTime.UtcNow.AddDays(-50).AddYears(2),
                    MonthlyRent = 45000m,
                    SecurityDeposit = 90000m,
                    TokenPaid = 15000m,
                    PaymentFrequency = PaymentFrequency.Monthly,
                    TermsAndConditions = "Standard Gujarat Residential Tenancy Lease Agreement under the Model Tenancy Act (Gujarat). Rent is payable on or before the 5th of each calendar month. Security deposit held in escrow. Tenant responsible for electricity and society maintenance charges. Landlord maintains structural integrity, water supply, and society amenities.",
                    Status = ContractStatus.Active,
                    DigitalSignature = "SIG-SHA256-" + Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper(),
                    GeneratedAt = DateTime.UtcNow.AddDays(-51)
                }
            };
            context.RentalContracts.AddRange(contracts);

            context.SaveChanges();
        }
    }
}

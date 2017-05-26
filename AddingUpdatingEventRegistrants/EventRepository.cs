// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventRepository.cs" company="U.S. Naval Academy Alumni Association & Foundation">
//   2017
// </copyright>
// <summary>
//   Defines the SampleEventsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AddingUpdatingEventRegistrants
{
    using System;

    public class SampleEventsRepository
    {
        public bool AddParticipant(Registrant part, ConstituentPaymentTransaction paymentTransaction)
        {
            Guid RegistrantIDMaster = Guid.NewGuid();
            var registrantData = RegistrantUnifiedAddForm.LoadData(Provider, part.EventId.ToString());
            registrantData.CONSTITUENTID = part.Id;
            registrantData.DATEPURCHASED = DateTime.Now;
            registrantData.SINGLEEVENTREGISTRATIONS =
                new RegistrantUnifiedAddFormData.SINGLEEVENTREGISTRATIONS_DATAITEM[part.Units.Count];
            registrantData.REGISTRATIONTYPECODE =
                RegistrantUnifiedAddFormData.REGISTRATIONTYPECODEVALUES.Preregistration;

            var registrants = new RegistrantUnifiedAddFormData.SINGLEEVENTREGISTRATIONS_DATAITEM[part.Units.Count];
            var i = 0;
            var mappings = new RegistrantUnifiedAddFormData.REGISTRANTMAPPINGS_DATAITEM[part.Units.Count];
            foreach (var unit in part.Units)
            {
                var registrant = new RegistrantUnifiedAddFormData.SINGLEEVENTREGISTRATIONS_DATAITEM();
                if (i == 0)
                {
                    registrant = new RegistrantUnifiedAddFormData.SINGLEEVENTREGISTRATIONS_DATAITEM
                                     {
                                         SINGLEEVENTREGISTRATIONID = RegistrantIDMaster,
                                         SINGLEEVENTREGISTRATIONEVENTID = part.EventId,
                                         SINGLEEVENTREGISTRATIONEVENTPRICEID = unit.Id,
                                         QUANTITY = part.Units.Count,
                                         REGISTRATIONCOUNT = part.Units.Count,
                                         AMOUNT = unit.Price * part.Units.Count,
                                         RECEIPTAMOUNT = 0

                                     };
                }
                else
                {
                    registrant = new RegistrantUnifiedAddFormData.SINGLEEVENTREGISTRATIONS_DATAITEM
                                     {
                                         SINGLEEVENTREGISTRATIONID = Guid.NewGuid(),
                                         SINGLEEVENTREGISTRATIONEVENTID = part.EventId,
                                         SINGLEEVENTREGISTRATIONEVENTPRICEID = unit.Id,
                                         QUANTITY = 0,
                                         REGISTRATIONCOUNT = 0,
                                         AMOUNT = unit.Price,
                                         RECEIPTAMOUNT = 0
                                     };
                }

                registrants[i] = registrant;

                var mapping = new RegistrantUnifiedAddFormData.REGISTRANTMAPPINGS_DATAITEM();
                if (i == 0)
                {
                    mapping = new RegistrantUnifiedAddFormData.REGISTRANTMAPPINGS_DATAITEM
                                  {

                                      EVENTID = part.EventId,
                                      EVENTPRICEID = unit.Id,
                                      GUESTCONSTITUENTID = part.Id,
                                      REGISTRATIONSCOLLECTIONID = RegistrantIDMaster,
                                      REGISTRANTWAIVEBENEFITS = new[] { new RegistrantUnifiedAddFormData.REGISTRANTMAPPINGS_DATAITEM.REGISTRANTWAIVEBENEFITS_DATAITEM() { EVENTID = part.Id, WAIVEBENEFITS = false } },
                                      HASNOTIFICATIONS = false,

                                  };
                }
                else
                {
                    mapping = new RegistrantUnifiedAddFormData.REGISTRANTMAPPINGS_DATAITEM
                                  {

                                      EVENTID = part.EventId,
                                      EVENTPRICEID = unit.Id,
                                      GUESTCONSTITUENTID = new Guid("c776dab5-65b2-4258-adae-ae396d28e251"),
                                      REGISTRATIONSCOLLECTIONID = RegistrantIDMaster,
                                      REGISTRANTWAIVEBENEFITS = new[] { new RegistrantUnifiedAddFormData.REGISTRANTMAPPINGS_DATAITEM.REGISTRANTWAIVEBENEFITS_DATAITEM() { EVENTID = part.Id, WAIVEBENEFITS = false } },
                                      HASNOTIFICATIONS = false,

                                  };
                }
                mappings[i] = mapping;

                i++;
            }
            registrantData.SINGLEEVENTREGISTRATIONS = registrants;
            registrantData.REGISTRANTMAPPINGS = mappings;


            var results = registrantData.Save(Provider);

            if (part.NameTag != null)
            {
                AddRegistrantAttribute(part, RegistrantAttributeType.NameTag, new Guid(results));
            }
            if (part.NameTags != null)
            {
                if (part.NameTagEventId != Guid.Empty)
                {
                    var primaryRegistrantId = GetRegistrantId(part.BackOfficeGuid, part.NameTagEventId);
                    if (CheckIfRegistrantAttributeExists(primaryRegistrantId.ToString(), RegistrantAttributeType.NameTag))
                        EditRegistrantAttribute(part, RegistrantAttributeType.NameTags, "nametag", primaryRegistrantId);
                    else
                        AddRegistrantAttribute(part, RegistrantAttributeType.NameTags, primaryRegistrantId);
                }

            }

            if (part.FourSome != null)
            {
                AddRegistrantAttribute(part, RegistrantAttributeType.Foursome, new Guid(results));
            }

            if (paymentTransaction != null && paymentTransaction.AmountPaid > 0)
            {
                paymentTransaction.ApplicationId = new Guid(results);
                var u = new UtilityRepository();
                u.AddConstituentPayment(paymentTransaction);
            }

            return true;
        }

        public bool UpdateParticipant(Registrant part, ConstituentPaymentTransaction paymentTransaction)
        {
            var registrantData = RegistrantUnifiedEditForm.LoadData(Provider, part.Id.ToString());
            var RegistrantID = new Guid(registrantData.RecordID);

            var registrants = new RegistrantUnifiedEditFormData.SINGLEEVENTREGISTRATIONS_DATAITEM[part.Units.Count + 1];
            var mappings = new RegistrantUnifiedEditFormData.REGISTRANTMAPPINGS_DATAITEM[part.Units.Count + 1];
            var i = 0;
            foreach (var unit in part.Units)
            {

                var mainRegistrant = new RegistrantUnifiedEditFormData.SINGLEEVENTREGISTRATIONS_DATAITEM();
                var registrant = new RegistrantUnifiedEditFormData.SINGLEEVENTREGISTRATIONS_DATAITEM();
                if (i == 0)
                {
                    mainRegistrant = new RegistrantUnifiedEditFormData.SINGLEEVENTREGISTRATIONS_DATAITEM
                                         {
                                             SINGLEEVENTREGISTRATIONID = RegistrantID,
                                             SINGLEEVENTREGISTRATIONEVENTID = part.EventId,
                                             SINGLEEVENTREGISTRATIONEVENTPRICEID = unit.Id,

                                             QUANTITY = part.Units.Count,
                                             REGISTRATIONCOUNT = part.Units.Count,
                                             AMOUNT = unit.Price * (part.Units.Count),
                                             RECEIPTAMOUNT = 0

                                         };
                    registrants[0] = mainRegistrant;
                    registrant = new RegistrantUnifiedEditFormData.SINGLEEVENTREGISTRATIONS_DATAITEM
                                     {
                                         SINGLEEVENTREGISTRATIONID = Guid.NewGuid(),
                                         SINGLEEVENTREGISTRATIONEVENTID = part.EventId,
                                         SINGLEEVENTREGISTRATIONEVENTPRICEID = unit.Id,
                                         QUANTITY = 0,
                                         REGISTRATIONCOUNT = 0,
                                         AMOUNT = unit.Price,
                                         RECEIPTAMOUNT = 0
                                     };
                    registrants[1] = registrant;
                }
                else
                {
                    registrant = new RegistrantUnifiedEditFormData.SINGLEEVENTREGISTRATIONS_DATAITEM
                                     {
                                         SINGLEEVENTREGISTRATIONID = Guid.NewGuid(),
                                         SINGLEEVENTREGISTRATIONEVENTID = part.EventId,
                                         SINGLEEVENTREGISTRATIONEVENTPRICEID = unit.Id,
                                         QUANTITY = 0,
                                         REGISTRATIONCOUNT = 0,
                                         AMOUNT = unit.Price,
                                         RECEIPTAMOUNT = 0
                                     };
                    registrants[i + 1] = registrant;
                }

                var mainMapping = new RegistrantUnifiedEditFormData.REGISTRANTMAPPINGS_DATAITEM();
                var mapping = new RegistrantUnifiedEditFormData.REGISTRANTMAPPINGS_DATAITEM();
                if (i == 0)
                {
                    mainMapping = new RegistrantUnifiedEditFormData.REGISTRANTMAPPINGS_DATAITEM
                                      {

                                          EVENTID = part.EventId,
                                          EVENTPRICEID = unit.Id,
                                          GUESTCONSTITUENTID = part.BackOfficeGuid,

                                          REGISTRANTWAIVEBENEFITS = new[] { new RegistrantUnifiedEditFormData.REGISTRANTMAPPINGS_DATAITEM.REGISTRANTWAIVEBENEFITS_DATAITEM() { EVENTID = part.Id, WAIVEBENEFITS = false } },
                                          HASNOTIFICATIONS = false,

                                      };
                    mappings[0] = mainMapping;
                    mapping = new RegistrantUnifiedEditFormData.REGISTRANTMAPPINGS_DATAITEM
                                  {

                                      EVENTID = part.EventId,
                                      EVENTPRICEID = unit.Id,
                                      GUESTCONSTITUENTID = new Guid("c776dab5-65b2-4258-adae-ae396d28e251"),
                                      REGISTRATIONSCOLLECTIONID = RegistrantID,
                                      REGISTRANTWAIVEBENEFITS = new[] { new RegistrantUnifiedEditFormData.REGISTRANTMAPPINGS_DATAITEM.REGISTRANTWAIVEBENEFITS_DATAITEM() { EVENTID = part.Id, WAIVEBENEFITS = false } },
                                      HASNOTIFICATIONS = false,

                                  };
                    mappings[1] = mapping;
                }
                else
                {
                    mapping = new RegistrantUnifiedEditFormData.REGISTRANTMAPPINGS_DATAITEM
                                  {

                                      EVENTID = part.EventId,
                                      EVENTPRICEID = unit.Id,
                                      GUESTCONSTITUENTID = new Guid("c776dab5-65b2-4258-adae-ae396d28e251"),
                                      REGISTRATIONSCOLLECTIONID = RegistrantID,
                                      REGISTRANTWAIVEBENEFITS = new[] { new RegistrantUnifiedEditFormData.REGISTRANTMAPPINGS_DATAITEM.REGISTRANTWAIVEBENEFITS_DATAITEM() { EVENTID = part.Id, WAIVEBENEFITS = false } },
                                      HASNOTIFICATIONS = false,

                                  };
                    mappings[i + 1] = mapping;
                }


                i++;

            }

            registrantData.SINGLEEVENTREGISTRATIONS = registrants;

            registrantData.REGISTRANTMAPPINGS = mappings;

            var results = registrantData.Save(Provider);

            if (part.NameTag != null)
            {
                AddRegistrantAttribute(part, RegistrantAttributeType.NameTag, RegistrantID);
            }
            if (part.NameTagEventId != Guid.Empty)
            {
                var primaryRegistrantId = GetRegistrantId(part.BackOfficeGuid, part.NameTagEventId);
                if (CheckIfRegistrantAttributeExists(primaryRegistrantId.ToString(), RegistrantAttributeType.NameTag))
                    EditRegistrantAttribute(part, RegistrantAttributeType.NameTags, "nametag", primaryRegistrantId);
                else
                    AddRegistrantAttribute(part, RegistrantAttributeType.NameTags, primaryRegistrantId);
            }

            if (part.FourSome != null)
            {
                AddRegistrantAttribute(part, RegistrantAttributeType.Foursome, RegistrantID);
            }

            if (paymentTransaction != null && paymentTransaction.AmountPaid > 0)
            {
                paymentTransaction.ApplicationId = new Guid(results);

                var u = new UtilityRepository();
                u.AddConstituentPayment(paymentTransaction);
            }

            Provider.EndSession();
            return true;
        }
    }
}
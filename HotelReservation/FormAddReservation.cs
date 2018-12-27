﻿using BusinessAccessLayer.Services;
using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class FormAddReservation : Form
    {
        BookingService bookingService = new BookingService();
        static string connectionString = ConfigurationManager.ConnectionStrings["HotelReservationConStr"].ConnectionString;
        UserRepository userRepository = new UserRepository(connectionString);
        BookingRepository bookingRepository = new BookingRepository(connectionString);
        int roomId;
        public FormAddReservation()
        {
            InitializeComponent();
        }
        public int RoomId
        {
            set
            {
                roomId = value;
            }
        }

        private void FormAddReservation_Shown(object sender, EventArgs e)
        {
            listViewUsers.Items.Clear();
            var allUsers = userRepository.GetAllUsers();
            foreach (var user in allUsers)
            {
                ListViewItem listViewUserRecord = new ListViewItem(user.Id.ToString());
                listViewUserRecord.SubItems.Add(user.UserName);
                listViewUsers.Items.Add(listViewUserRecord);
            }
        }



        private void buttonAddReservation_Click(object sender, EventArgs e)
        {
            bookingService.GetBookingDatesRange(dateTimePickerStart.Value, dateTimePickerEnd.Value);
            if (ValidationService.ValidateBookingDates(dateTimePickerStart.Value, dateTimePickerEnd.Value))
            {
                if (listViewUsers.SelectedItems.Count != 0)
                {
                    ListViewItem item = listViewUsers.SelectedItems[0];
                    Booking booking = new Booking()
                    {
                        StartDate = dateTimePickerStart.Value,
                        EndDate = dateTimePickerEnd.Value,
                        UserId = Convert.ToInt32(item.Text),
                        RoomId = roomId
                    };
                    bookingRepository.AddBooking(booking);
                    MessageBox.Show("New record has added");
                    this.Close();
                }
                else
                {
                    labelError.Visible = true;
                }
            }
            else
                labelDatesValidation.Visible = true;

        }

        private void listViewUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelError.Visible = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (listViewUsers.SelectedItems.Count != 0)
            {
                ListViewItem item = listViewUsers.SelectedItems[0];
                bookingRepository.CancelBooking(item.Text);
                MessageBox.Show("The reservation has been canceled");
                this.Close();
            }
            else
            {
                labelError.Visible = true;
            }

        }
    }
}

namespace OSGroupMembershipTool
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        

        internal List<string> cloneFromUserMemberships = new List<string>();

        internal List<string> cloneToUserMemberships = new List<string>();

        internal List<string> allNewGroupMemberships = new List<string>();

        internal List<string> membershipDifferences = new List<string>();

        string domainName;


        protected void GetCloneFromUserMemberships()
        {
            cloneFromUserMemberships.Clear();

            UserPrincipal user = UserPrincipal.FindByIdentity(ldapSearcher, User_to_Clone_From.Text);

            Membership_List_User.Content = "Showing Group Memberships for " + user;

            foreach (GroupPrincipal per in user.GetGroups())
                if (user != null)
                {
                    cloneFromUserMemberships.Add(per.ToString());

                    cloneFromUserMemberships.Sort((x, y) => string.Compare(x, y));
                }
            user.Dispose();
        }

        protected void GetCloneToUserMemberships()
        {
            cloneToUserMemberships.Clear();

            UserPrincipal user = UserPrincipal.FindByIdentity(ldapSearcher, User_to_Clone_To.Text);

            Membership_List_User.Content = "Showing Group Memberships for " + user;

            foreach (GroupPrincipal per in user.GetGroups())
                if (user != null)
                {
                    cloneToUserMemberships.Add(per.ToString());

                    cloneToUserMemberships.Sort((x, y) => string.Compare(x, y));
                }
            user.Dispose();
        }

        private void RemoveAllCloneToMemberships()
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("You Are About To Remove All Memberships From " + User_to_Clone_To.Text + " Continue?", "Add New Memberships", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                for (int i = 0; i < cloneToUserMemberships.Count; i++)
                {
                    try
                    {
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(ldapSearcher, cloneToUserMemberships[i]);
                        group.Members.Remove(ldapSearcher, IdentityType.SamAccountName, User_to_Clone_To.Text);
                        group.Save();
                    }
                    catch (System.DirectoryServices.DirectoryServicesCOMException E)
                    {
                        User_Memberships.Text += E.Message.ToString();
                    }
                }
            }
        }

        private void RemoveAndTransferMemberships()
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("You Are About To Replace All Memberships For " + User_to_Clone_To.Text + " With Memberships From " + User_to_Clone_From.Text + " Continue?", "Add New Memberships", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                for (int i = 0; i < cloneToUserMemberships.Count; i++)
                {
                    try
                    {
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(ldapSearcher, cloneToUserMemberships[i]);
                        if (cloneToUserMemberships[i] != "Domain Users")
                        {
                            group.Members.Remove(ldapSearcher, IdentityType.SamAccountName, User_to_Clone_To.Text);
                            group.Save();
                        }

                    }
                    catch (System.DirectoryServices.DirectoryServicesCOMException E)
                    {
                        User_Memberships.Text += E.Message.ToString();
                    }
                }
            }
        }

        private void MembershipDifferences()
        {
            membershipDifferences.Clear();

            Membership_List_User.Content = "Showing Group Memberships To Clone ";

            membershipDifferences = cloneFromUserMemberships.Except(cloneToUserMemberships).ToList();

            //User_Memberships.Text += cloneFromUserMemberships[i] + "\n";
            //Console.WriteLine(string.Join(Environment.NewLine, membershipDifferences));
            User_Memberships.Text += string.Join(Environment.NewLine, membershipDifferences);
        }

        private void CloneMemberships()
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("You Are About To Clone All Memberships From " + User_to_Clone_From.Text + " to " + User_to_Clone_To.Text + " Continue?", "Add New Memberships", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {

                for (int i = 0; i < membershipDifferences.Count; i++)
                {
                    try
                    {
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(ldapSearcher, membershipDifferences[i]);
                        group.Members.Add(ldapSearcher, IdentityType.SamAccountName, User_to_Clone_To.Text);
                        group.Save();

                    }
                    catch (System.DirectoryServices.DirectoryServicesCOMException E)
                    {
                        User_Memberships.Text += E.Message.ToString();
                    }
                }

            }
            else
            {
                MessageBoxResult tryAgain = MessageBox.Show("Try Operation Again");
            }
        }

        private void Print_Clone_From_Members_Click(object sender, RoutedEventArgs e)
        {
            User_Memberships.Text = string.Empty;
            GetCloneFromUserMemberships();

            for (int i = 0; i < cloneFromUserMemberships.Count; i++)
            {
                User_Memberships.Text += cloneFromUserMemberships[i] + "\n";
            }
        }

        private void Print_Clone_To_Members_Click(object sender, RoutedEventArgs e)
        {
            User_Memberships.Text = string.Empty;
            GetCloneToUserMemberships();

            for (int i = 0; i < cloneToUserMemberships.Count; i++)
            {
                User_Memberships.Text += cloneToUserMemberships[i] + "\n";
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            User_Memberships.Text = string.Empty;
            //GetCloneFromUserMemberships();
            //GetCloneToUserMemberships();
            MembershipDifferences();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            User_Memberships.Text = string.Empty;
            GetCloneFromUserMemberships();
            GetCloneToUserMemberships();
            MembershipDifferences();
            CloneMemberships();
        }

        private void Remove_And_Transfer_Click(object sender, RoutedEventArgs e)
        {
            User_Memberships.Text = string.Empty;
            GetCloneFromUserMemberships();
            GetCloneToUserMemberships();
            MembershipDifferences();
            RemoveAndTransferMemberships();
            CloneMemberships();
            for (int i = 0; i < cloneToUserMemberships.Count; i++)
            {
                User_Memberships.Text += cloneToUserMemberships[i] + "\n";
            }
        }

        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            Domain_Name.Text = domainName;
            User_Memberships.Text += domainName;
        }
    }
}

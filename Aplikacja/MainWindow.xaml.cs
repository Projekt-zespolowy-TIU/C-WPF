using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;//Regex

namespace Aplikacja
{

    public partial class MainWindow : Window
    {
        //TekstBoxy z 4 czesciami podanego Ip oraz Maska 
        List<TextBox> textBoxes;
        Regex regex = new Regex("[^0-9]+");
        //Ktory TextBox z listy jest aktualnie aktywny
        int focused = 0;
        public MainWindow()
        {
            InitializeComponent();

            //Ustawienie początkowej widocznosci siatek//Ipv4-DomyslnieWlaczone
            _gridIPv4.Visibility = Visibility.Visible;
            _gridIPv6.Visibility = Visibility.Collapsed;

            //Dodawanie tekstboxow ze sceny do tablicy
            textBoxes = new List<TextBox>();
            foreach (TextBox text in _gridIPv4.Children.OfType<TextBox>())
            {
                textBoxes.Add(text);
            }
            textBoxes[focused].Focus();

            #region Dodanie nowyej funkcji do tekstboxow sprawdzajacych poprawnosc kopjowanych danych
            DataObject.AddPastingHandler(_textbox1, OnPaste);
            DataObject.AddPastingHandler(_textbox2, OnPaste);
            DataObject.AddPastingHandler(_textbox3, OnPaste);
            DataObject.AddPastingHandler(_textbox4, OnPaste);
            DataObject.AddPastingHandler(_textboxMaska, OnPaste);
            #endregion

            #region Dodanie danych do comboBoxa z wszyskimi mozliwymi maskami oraz ustawienie wartosci domyslnej
            _comboBox_Maska.Items.Add("0.0.0.0");
            _comboBox_Maska.Items.Add("128.0.0.0");
            _comboBox_Maska.Items.Add("192.0.0.0");
            _comboBox_Maska.Items.Add("224.0.0.0");
            _comboBox_Maska.Items.Add("240.0.0.0");
            _comboBox_Maska.Items.Add("248.0.0.0");
            _comboBox_Maska.Items.Add("252.0.0.0");
            _comboBox_Maska.Items.Add("254.0.0.0");
            _comboBox_Maska.Items.Add("255.0.0.0");
            _comboBox_Maska.Items.Add("255.128.0.0");
            _comboBox_Maska.Items.Add("255.192.0.0");
            _comboBox_Maska.Items.Add("255.224.0.0");
            _comboBox_Maska.Items.Add("255.240.0.0");
            _comboBox_Maska.Items.Add("255.248.0.0");
            _comboBox_Maska.Items.Add("255.252.0.0");
            _comboBox_Maska.Items.Add("255.254.0.0");
            _comboBox_Maska.Items.Add("255.255.0.0");
            _comboBox_Maska.Items.Add("255.255.128.0");
            _comboBox_Maska.Items.Add("255.255.192.0");
            _comboBox_Maska.Items.Add("255.255.224.0");
            _comboBox_Maska.Items.Add("255.255.240.0");
            _comboBox_Maska.Items.Add("255.255.248.0");
            _comboBox_Maska.Items.Add("255.255.252.0");
            _comboBox_Maska.Items.Add("255.255.254.0");
            _comboBox_Maska.Items.Add("255.255.255.0");
            _comboBox_Maska.Items.Add("255.255.255.128");
            _comboBox_Maska.Items.Add("255.255.255.192");
            _comboBox_Maska.Items.Add("255.255.255.224");
            _comboBox_Maska.Items.Add("255.255.255.240");
            _comboBox_Maska.Items.Add("255.255.255.248");
            _comboBox_Maska.Items.Add("255.255.255.252");
            _comboBox_Maska.Items.Add("255.255.255.254");
            _comboBox_Maska.Items.Add("255.255.255.255");
            _comboBox_Maska.SelectedIndex = 24;
            #endregion 
        }

        //Sprawdzenie poprawnosci kopiowanych danuch
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            if (e.Handled = regex.IsMatch(text))
                e.CancelCommand();
        }

        //Przeniesienie Caret'y do nastepnego boxa jesli obeny jest pelny
        private void Textbox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox box = sender as TextBox;

            if (box.GetLineLength(0) == box.MaxLength)
            {
                if (focused >= 0 || focused < textBoxes.Count - 1)
                    textBoxes[focused + 1].Focus();
            }
        }

        //Przelaczanie siatek z ipv4 i ipv6
        private void RadioIPv6_Click(object sender, RoutedEventArgs e)
        {
            _gridIPv4.Visibility = Visibility.Collapsed;
            _gridIPv6.Visibility = Visibility.Visible;
        }
        private void RadioIPv4_Click(object sender, RoutedEventArgs e)
        {
            _gridIPv4.Visibility = Visibility.Visible;
            _gridIPv6.Visibility = Visibility.Collapsed;
        }

        //Wyowlanie funkcji obliczajacej po kliknieciu buttona Oblicz
        private void Button_Oblicz_Click(object sender, RoutedEventArgs e)
        {
            CountIPv4();
        }

        //Sprawdzenie czy wpisywane dane do tekstboxow to liczby
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
        }

        //Czyszczenie zawartosci textbokow i listboxa oraz ustawienie znaku Caret'y na pierwszym textboxie
        private void Button_Wyczysc_Click(object sender, RoutedEventArgs e)
        {
            focused = 0;
            textBoxes[focused].Focus();
            for (int i = 0; i < textBoxes.Count - 1; i++)
            {
                textBoxes[i].Clear();
            }
            _listbox_4.Items.Clear();
        }

        //Funkcja odpowiedzialana za obliczanie wszystkich adresow
        private void CountIPv4()
        {
            _listbox_4.Items.Clear();

            //Sprawdzenie czy w textboxach sa liczby z przedzialu 0-255, maska 0-32
            if (int.TryParse(_textbox1.Text, out int texbox1) && int.TryParse(_textbox2.Text, out int texbox2)
                && int.TryParse(_textbox3.Text, out int texbox3) && int.TryParse(_textbox4.Text, out int texbox4)
                && int.TryParse(_textboxMaska.Text, out int texboxMaska))
                if (texbox1 >= 0 && texbox1 <= 255 && texbox2 >= 0 && texbox2 <= 255 && texbox3 >= 0 && texbox3 <= 255 && texbox4 >= 0
                    && texbox4 <= 255 && texboxMaska >= 0 && texboxMaska <= 32)
                {
                    IPv4 A = new IPv4(_comboBox_Maska, _textbox1, _textbox2, _textbox3, _textbox4);

                    _listbox_4.Items.Add("Adres IPv4 : " + A.address);
                    _listbox_4.Items.Add("Adres IPv4 Binarnie : " + A.address_binary);

                    _listbox_4.Items.Add("Maska: " + A.mask);
                    _listbox_4.Items.Add("Maska binarnie: " + A.mask_binary);

                    _listbox_4.Items.Add("Adres sieci : " + A.networkAddress);
                    _listbox_4.Items.Add("Adres sieci binarnie: " + A.networkAddress_binary);

                    _listbox_4.Items.Add("Adres rozgłoszeniowy : " + A.bradcastAddress);
                    _listbox_4.Items.Add("Adres rozgłoszeniowy binarnie: " + A.bradcastAddress_binary);

                    _listbox_4.Items.Add("Liczba hostow: " + A.numberOfHosts);

                    A.ListOfSubnetworks(int.Parse(_comboBox_IloscAdresow.SelectedIndex.ToString()), _listbox_4);
                }
                else
                    _listbox_4.Items.Add("Niepoprawne Dane");
        }

        //Obsluga aplikacji za pomoca klawiaturu,zmienianie tektboxow strzalkami oraz enter uruchamia obliczanie
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CountIPv4();

            if ((textBoxes[focused].CaretIndex == 0) && (e.Key == Key.Back || e.Key == Key.Left) && (focused > 0))
                textBoxes[--focused].Focus();

            if ((textBoxes[focused].CaretIndex == textBoxes[focused].Text.Length) && ((e.Key == Key.Right) && (focused < 4)))
                textBoxes[++focused].Focus();
        }

        //Zmiana parametru focused podczas przechodzenia miedzy textboxami,strzalkami badz po nacisnieciu na textboxa myszka
        private void Textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Name == box.Name)
                    focused = i;
            }
        }

        //Zmiana zawartosci comboBoxa maski przy zmianie textBoxa maski
        private void TextboxMaska_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (int.TryParse(box.Text, out int maskNumber))
                _comboBox_Maska.SelectedIndex = maskNumber;
            //else throw error blad konwersji
        }

        //Zmiana zawartosci comboBox'ow z ilosciami adresow i podsieci podczas zmieniania maski
        private void ComboBox_Maska_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _textboxMaska.Text = _comboBox_Maska.SelectedIndex.ToString();
            _comboBox_IloscAdresow.Items.Clear();
            for (int i = 32 - int.Parse(_textboxMaska.Text); i >= 0; i--)
            {
                _comboBox_IloscAdresow.Items.Add((long)(Math.Pow(2, i)));
            }
            _comboBox_IloscAdresow.SelectedIndex = 0;


            //Mozna zmienic aby obracały powyższego comboBoxa zamiast obliczac.
            _comboBox_IloscPodsieci.Items.Clear();
            for (int i = 0; i < _comboBox_IloscAdresow.Items.Count; i++)
            {
                _comboBox_IloscPodsieci.Items.Add((long)Math.Pow(2, i));
            }
            _comboBox_IloscPodsieci.SelectedIndex = 0;
        }

        //Powiazanie ze soba comboBoxow odpowiedzialnych za IloscAdresow i IloscPodsieci
        private void ComboBox_IloscAdresow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _comboBox_IloscPodsieci.SelectedIndex = _comboBox_IloscAdresow.SelectedIndex;
        }
        private void ComboBox_IloscPodsieci_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _comboBox_IloscAdresow.SelectedIndex = _comboBox_IloscPodsieci.SelectedIndex;
        }

    }

    public class IPv4
    {
        public string address { get; private set; }//adres podany przez uzytkownika
        public string mask { get; private set; }//maska wybrana przez uzytkownika
        public string mask_binary { get; private set; }
        public string address_binary { get; private set; }
        public string networkAddress { get; private set; }
        public string networkAddress_binary { get; private set; }
        public string bradcastAddress { get; private set; }
        public string bradcastAddress_binary { get; private set; }
        public long numberOfHosts { get; private set; }

        public IPv4(ComboBox combo, params TextBox[] textBoxes)
        {
            address = GivenAddress(textBoxes);
            address_binary = DecimalToBinary(address);

            mask = GivenMask(combo);
            mask_binary = DecimalToBinary(mask);

            networkAddress_binary = NetworkAddress_Binary(address_binary, mask_binary);
            networkAddress = BinaryToDecimal(networkAddress_binary);

            bradcastAddress_binary = BroadcastAddress_Binary(address_binary, mask_binary);
            bradcastAddress = BinaryToDecimal(bradcastAddress_binary);

            numberOfHosts = NumberOfHosts(mask_binary);
        }

        //text postaci 255 zamienia na 11111111
        private string ToBinary(string text)
        {
            int number;
            string bin = "";
            if (int.TryParse(text, out number))
            {
                //Zamiana na liczbe binarna
                while (number > 0)
                {
                    if (number % 2 == 0)
                    {
                        bin += "0";
                        number = number / 2;
                    }
                    else
                    {
                        bin += "1";
                        number = number / 2;
                    }
                }
                //Uzupelnianie do 8 bitow
                while (true)
                {
                    if (bin.Length < 8)
                        bin += "0";
                    else
                        break;
                }
            }
            //else Blad Konwersji
            //obracanie tablicy
            char[] charArray = bin.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        //text postaci 11111111.11111111.11111111.11111111 zamienia na 255.255.255.255
        private string BinaryToDecimal(string text)
        {
            string dec = "";
            int decNumber = 0;
            int power_of = 7;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].ToString() == ".")
                {
                    dec += decNumber.ToString();
                    decNumber = 0;
                    power_of = 7;
                    dec += ".";
                }
                else
                {
                    decNumber += int.Parse(text[i].ToString()) * (int)Math.Pow(2, power_of);
                    power_of--;
                    if (i == text.Length - 1)
                        dec += decNumber.ToString();
                }
            }
            return dec;
        }

        //text postaci 255.255.255.255 zamienia na 11111111.11111111.11111111.11111111
        private string DecimalToBinary(string text)
        {
            string number = null;//wycinek postaci 11111111
            string binary = "";
            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i] == '.') || (i == text.Length))
                {
                    binary += ToBinary(number);
                    if (i != text.Length - 1)
                        binary += ".";
                    number = "";
                }
                else
                {
                    number += text[i];
                    if (i == text.Length - 1)
                        binary += ToBinary(number);
                }
            }
            return binary;
        }

        //Skleja text z podanych textBoxow i zwraca stringa postaci 255.255.255.255
        private string GivenAddress(params TextBox[] textBoxes)
        {
            string address = "";
            for (int i = 0; i < textBoxes.Length; i++)
            {
                address += textBoxes[i].Text.ToString();
                if (i != textBoxes.Length - 1)
                    address += ".";
            }
            return address;
        }

        //Zamienia text z ComboBoxa na stringa
        private string GivenMask(ComboBox cbox)
        {
            return cbox.SelectedItem.ToString();
        }

        //Zwraca adres sieci w postaci binarnej przy podanym na wejsciu adresie i masce w postaci binarnej
        private string NetworkAddress_Binary(string address_binary, string mask_binary)
        {
            string n_address_binary = "";
            for (int i = 0; i < mask_binary.Length; i++)
            {
                if (mask_binary[i] == address_binary[i])
                    n_address_binary += mask_binary[i];
                else n_address_binary += "0";
            }
            return n_address_binary;
        }

        //Zwraca adres rozgłoszeniowy w postaci binarnej przy podanym na wejsci adresie sieci oraz maski w postaci binarnej
        private string BroadcastAddress_Binary(string networkAddress_binary, string mask_binary)
        {
            string broadcast_binary = "";
            for (int i = 0; i < mask_binary.Length; i++)
            {
                if (mask_binary[i].ToString() == ".")
                    broadcast_binary += ".";
                else if (mask_binary[i].ToString() == "1")
                    broadcast_binary += networkAddress_binary[i];
                else broadcast_binary += "1";
            }
            return broadcast_binary;
        }

        //Zwraca zminna typu long zawierajaca liczbe hostow w sieci o podanej masce (binarnie)
        private long NumberOfHosts(string mask_binary)
        {
            int maskNumber = 0;//ilosc jedynek w masce
            for (int i = 0; i < mask_binary.Length; i++)
            {
                if (mask_binary[i].ToString() == "1")
                    maskNumber++;
            }
            return (long)(Math.Pow(2, 32 - maskNumber) - 2);
        }

        //Funkcja dodaje 1 do podanego textu postaci 11111111.11111111.11111111.00000000 i zwraca text rowniez w postaci binarnej
        private string AddOneBinary(string text)
        {
            char[] textCh = text.ToCharArray();
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if (textCh[i] == '.')
                    continue;
                if (textCh[i] == '0')
                {
                    textCh[i] = '1';
                    break;
                }
                else
                    textCh[i] = '0';
            }
            text = new string(textCh);
            return text;
        }

        //Oblicza adresy podsieci dla podanej liczby porzyczonych bitow i ListBoxa w którym maja pojawić się informacje
        public void ListOfSubnetworks(int numberOfBorrowBits, ListBox listBox)
        {
            string newMask_Binary = String.Copy(mask_binary);
            char[] newMaskChar_Binary = newMask_Binary.ToCharArray();
            string newBroadcast_Binary;
            string newNetwork_Binary = String.Copy(networkAddress_binary);
            long newNumberOfHosts;

            listBox.Items.Add("");
            listBox.Items.Add("Konfiguracja przy podziale na " + (long)Math.Pow(2, numberOfBorrowBits) + " podsieci.");

            for (int i = 0, j = numberOfBorrowBits; i < newMask_Binary.Length && j > 0; i++)
            {
                if ((newMask_Binary[i].ToString() == "1") || (newMask_Binary[i].ToString() == "."))
                    continue;
                else
                {
                    newMaskChar_Binary[i] = '1';
                    j--;
                }
            }

            newMask_Binary = new string(newMaskChar_Binary);
            listBox.Items.Add("Maska dla kazdej z podsieci : " + BinaryToDecimal(newMask_Binary));

            newNumberOfHosts = NumberOfHosts(newMask_Binary);
            listBox.Items.Add("Liczba hostow dla kazdej z podsieci : " + newNumberOfHosts);

            //Wyświetlanie adresu sieci i rozgłoszeniowego dla pierwszych stu podsieci
            for (int i = 1; (i <= (long)Math.Pow(2, numberOfBorrowBits) && i <= 100); i++)
            {
                listBox.Items.Add("Adres podsieci nr" + i + " : " + BinaryToDecimal(newNetwork_Binary));

                newBroadcast_Binary = BroadcastAddress_Binary(newNetwork_Binary, newMask_Binary);
                listBox.Items.Add("Adres rozgłoszeniowy podsieci nr" + i + " : " + BinaryToDecimal(newBroadcast_Binary));

                //Obliczenie adresu dla nastepnej podsieci
                newNetwork_Binary = AddOneBinary(newBroadcast_Binary);
            }
        }

    }
}

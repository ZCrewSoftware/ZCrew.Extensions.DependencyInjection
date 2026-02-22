using System.Collections;

namespace Fixtures.LargeProject.Application.Services;

public class Service1 : IService35;

internal class Service2 : Service1, IService33;

internal class Service3 : IService3;

public class Service4 : IService26;

public class Service5 : IService14;

internal class Service6 : IService4, IEnumerable<Service6>, IEnumerable
{
    public IEnumerator<Service6> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service7 : IService36, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service8 : IService27, IAsyncDisposable, IEquatable<Service8>, IEnumerable<Service8>, IEnumerable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service8? other)
    {
        return false;
    }

    public IEnumerator<Service8> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service9 : IService49, IDisposable
{
    public void Dispose() { }
}

public class Service10 : IService48;

public class Service11 : IService25;

public class Service12 : IService16;

public class Service13 : IService17
{
    public void Dispose() { }
}

internal class Service14 : IService25;

internal class Service15 : IService28, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service16 : IService36, IAsyncDisposable, IEnumerable<Service16>, IEnumerable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public IEnumerator<Service16> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service17 : IService35, IDisposable, IAsyncDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service18 : IService45, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service19 : IService26;

public class Service20 : Service1, IService18;

public class Service21 : Service17, IService40;

internal class Service22 : IService44
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service23 : IService39;

internal class Service24 : IService38, IDisposable
{
    public void Dispose() { }
}

public class Service25 : IService13;

internal class Service26 : Service8, IService50;

public class Service27 : IService42
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service28 : IService2;

internal class Service29 : Service22, IService39, IEquatable<Service29>
{
    public bool Equals(Service29? other)
    {
        return false;
    }
}

public class Service30 : IService23, IAsyncDisposable, IEnumerable<Service30>, IEnumerable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public IEnumerator<Service30> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service31 : IService47;

public class Service32 : IService24;

public class Service33 : Service30, IService4;

public class Service34 : IService49;

internal class Service35 : Service25, IService44
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service36 : IService8, IEquatable<Service36>
{
    public void Dispose() { }

    public bool Equals(Service36? other)
    {
        return false;
    }
}

public class Service37 : IService49, IEnumerable<Service37>, IEnumerable
{
    public IEnumerator<Service37> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service38 : IService45;

public class Service39 : IService9, IAsyncDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service40 : IService30;

public class Service41 : IService45, IAsyncDisposable, IEquatable<Service41>, IEnumerable<Service41>, IEnumerable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service41? other)
    {
        return false;
    }

    public IEnumerator<Service41> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service42 : ServiceBase27, IService3, IEquatable<Service42>
{
    public bool Equals(Service42? other)
    {
        return false;
    }
}

public class Service43 : IService33, IDisposable
{
    public void Dispose() { }
}

public class Service44 : IService30;

public class Service45 : IService30;

public class Service46 : IService46;

public class Service47 : ServiceBase33, IService31;

public class Service48 : IService11;

public class Service49 : IService29;

public class Service50 : IService23;

internal class Service51 : IService17
{
    public void Dispose() { }
}

public class Service52 : IService36;

public class Service53 : IService5;

public class Service54 : IService31;

public class Service55 : IService2, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service56 : IService27, IEquatable<Service56>
{
    public bool Equals(Service56? other)
    {
        return false;
    }
}

public class Service57 : IService23;

public class Service58 : IService25;

internal class Service59 : Service13, IService3;

public class Service60 : IService22
{
    public void Dispose() { }
}

internal class Service61 : IService6, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service62 : IService43;

public class Service63 : ServiceBase20, IService25, IAsyncDisposable
{
    public override ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service64 : IService40;

public class Service65 : IService1;

internal class Service66 : IService32, IDisposable
{
    public void Dispose() { }
}

public class Service67 : IService19
{
    public void Dispose() { }
}

internal class Service68 : ServiceBase41, IService2;

internal class Service69 : IService4;

internal class Service70 : IService32;

public class Service71 : IService31;

public class Service72 : Service45, IService33;

internal class Service73 : IService21;

public class Service74 : ServiceBase25, IService44
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service75 : Service71, IService38;

internal class Service76 : ServiceBase36, IService8, IAsyncDisposable, IEnumerable<Service76>, IEnumerable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public IEnumerator<Service76> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service77 : IService33;

public class Service78 : IService30;

public class Service79 : IService33;

public class Service80 : IService43, IAsyncDisposable, IEnumerable<Service80>, IEnumerable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public IEnumerator<Service80> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service81 : IService9
{
    public void Dispose() { }
}

public class Service82 : IService19, IAsyncDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service83 : IService32;

public class Service84 : IService48, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service85 : IService20
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service86 : IService49, IEnumerable<Service86>, IEnumerable
{
    public IEnumerator<Service86> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service87 : IService34;

public class Service88 : IService38;

public class Service89 : IService33;

public class Service90 : IService5, IEnumerable<Service90>, IEnumerable
{
    public IEnumerator<Service90> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service91 : IService12;

internal class Service92 : ServiceBase42, IService40, IAsyncDisposable
{
    public override ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service93 : IService32;

public class Service94 : IService19, IAsyncDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service95 : IService49, IEquatable<Service95>
{
    public bool Equals(Service95? other)
    {
        return false;
    }
}

public class Service96 : IService38;

public class Service97 : IService3;

internal class Service98 : IService2;

public class Service99 : IService12;

public class Service100 : IService36;

internal class Service101 : IService17
{
    public void Dispose() { }
}

public class Service102 : IService42, IDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service103 : IService4, IDisposable
{
    public void Dispose() { }
}

public class Service104 : IService26;

public class Service105 : Service95, IService26;

public class Service106 : IService25;

public class Service107 : IService2;

public class Service108 : IService11;

public class Service109 : IService29;

internal class Service110 : IService12;

public class Service111 : IService23, IDisposable
{
    public void Dispose() { }
}

internal class Service112 : IService39;

internal class Service113 : IService47;

internal class Service114 : IService25;

public class Service115 : IService37, IDisposable
{
    public void Dispose() { }
}

internal class Service116 : IService42, IEquatable<Service116>
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service116? other)
    {
        return false;
    }
}

public class Service117 : IService40;

public class Service118 : IService28;

internal class Service119 : ServiceBase2, IService39, IDisposable, IAsyncDisposable, IEquatable<Service119>
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service119? other)
    {
        return false;
    }
}

public class Service120 : ServiceBase37, IService49, IEnumerable<Service120>, IEnumerable
{
    public IEnumerator<Service120> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service121 : ServiceBase9, IService4, IDisposable
{
    public override void Dispose() { }
}

internal class Service122 : IService6;

internal class Service123 : IService50;

public class Service124 : IService47;

internal class Service125 : IService20
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service126 : IService49, IEquatable<Service126>
{
    public bool Equals(Service126? other)
    {
        return false;
    }
}

public class Service127 : Service95, IService38;

internal class Service128 : IService26;

internal class Service129 : IService29;

internal class Service130 : IService8
{
    public void Dispose() { }
}

public class Service131 : IService15;

internal class Service132 : IService27;

public class Service133 : ServiceBase15, IService6
{
    public override void Dispose() { }
}

public class Service134 : IService10, IEquatable<Service134>
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service134? other)
    {
        return false;
    }
}

public class Service135 : Service104, IService48;

public class Service136 : Service48, IService7
{
    public void Dispose() { }
}

public class Service137 : IService45;

internal class Service138 : IService4, IDisposable, IAsyncDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service139 : IService6;

public class Service140 : IService31, IEquatable<Service140>
{
    public bool Equals(Service140? other)
    {
        return false;
    }
}

public class Service141 : ServiceBase44, IService31
{
    public override ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service142 : IService50, IEquatable<Service142>
{
    public bool Equals(Service142? other)
    {
        return false;
    }
}

internal class Service143 : ServiceBase48, IService21;

public class Service144 : Service124, IService18;

internal class Service145 : IService37;

public class Service146 : ServiceBase42, IService49, IDisposable
{
    public void Dispose() { }

    public override ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service147 : IService32, IEquatable<Service147>
{
    public bool Equals(Service147? other)
    {
        return false;
    }
}

public class Service148 : IService32, IDisposable, IEnumerable<Service148>, IEnumerable
{
    public void Dispose() { }

    public IEnumerator<Service148> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service149 : IService28;

public class Service150 : Service89, IService48, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service151 : IService25, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service152 : IService36, IEquatable<Service152>
{
    public bool Equals(Service152? other)
    {
        return false;
    }
}

internal class Service153 : IService34;

internal class Service154 : IService2;

public class Service155 : Service41, IService27, IDisposable
{
    public void Dispose() { }
}

internal class Service156 : IService16, IDisposable
{
    public void Dispose() { }
}

public class Service157 : IService40, IDisposable
{
    public void Dispose() { }
}

internal class Service158 : IService37;

public class Service159 : IService45, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service160 : IService26;

public class Service161 : IService17
{
    public void Dispose() { }
}

public class Service162 : IService40, IEnumerable<Service162>, IEnumerable
{
    public IEnumerator<Service162> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service163 : IService42
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service164 : IService37, IDisposable
{
    public void Dispose() { }
}

internal class Service165 : IService47, IEnumerable<Service165>, IEnumerable
{
    public IEnumerator<Service165> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service166 : Service161, IService9;

public class Service167 : ServiceBase1, IService50;

public class Service168 : IService40, IDisposable
{
    public void Dispose() { }
}

public class Service169 : IService38;

public class Service170 : Service36, IService7;

public class Service171 : IService24;

public class Service172 : IService17
{
    public void Dispose() { }
}

public class Service173 : IService3;

internal class Service174 : Service170, IService8;

public class Service175 : IService7
{
    public void Dispose() { }
}

public class Service176 : IService12;

internal class Service177 : IService38;

public class Service178 : IService46, IDisposable
{
    public void Dispose() { }
}

public class Service179 : IService49, IEnumerable<Service179>, IEnumerable
{
    public IEnumerator<Service179> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service180 : IService23;

public class Service181 : IService22, IEnumerable<Service181>, IEnumerable
{
    public void Dispose() { }

    public IEnumerator<Service181> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service182 : ServiceBase35, IService16;

public class Service183 : Service115, IService47;

public class Service184 : ServiceBase13, IService21, IEquatable<Service184>
{
    public bool Equals(Service184? other)
    {
        return false;
    }
}

internal class Service185 : IService28, IDisposable
{
    public void Dispose() { }
}

public class Service186 : IService10
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service187 : IService50;

internal static class Service188;

internal class Service189 : IService45, IEnumerable<Service189>, IEnumerable
{
    public IEnumerator<Service189> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service190 : IService2;

public class Service191 : IService18, IAsyncDisposable, IEnumerable<Service191>, IEnumerable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public IEnumerator<Service191> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service192 : IService28;

public class Service193 : IService9, IEquatable<Service193>
{
    public void Dispose() { }

    public bool Equals(Service193? other)
    {
        return false;
    }
}

internal class Service194 : IService7
{
    public void Dispose() { }
}

public class Service195 : Service60, IService10
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service196 : IService44
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service197 : IService49;

internal class Service198 : IService27;

public class Service199 : IService3;

public class Service200 : Service104, IService12, IDisposable
{
    public void Dispose() { }
}

internal class Service201 : IService6, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service202 : ServiceBase44, IService20, IEquatable<Service202>
{
    public override ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service202? other)
    {
        return false;
    }
}

public class Service203 : IService45, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service204 : IService27;

public class Service205 : ServiceBase35, IService40;

public class Service206 : ServiceBase8, IService18
{
    public override void Dispose() { }
}

public class Service207 : IService44
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service208 : IService11;

public class Service209 : Service196, IService44;

internal class Service210 : IService49, IEnumerable<Service210>, IEnumerable
{
    public IEnumerator<Service210> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service211 : IService15;

public class Service212 : IService35;

internal class Service213 : IService33, IEnumerable<Service213>, IEnumerable
{
    public IEnumerator<Service213> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service214 : IService6;

internal class Service215 : IService44
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service216 : IService42
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service217 : IService20, IDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service218 : IService10
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service219 : Service126, IService8, IEnumerable<Service219>, IEnumerable
{
    public void Dispose() { }

    public IEnumerator<Service219> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service220 : Service9, IService15;

public class Service221 : IService19
{
    public void Dispose() { }
}

public class Service222 : Service9, IService31, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service223 : IService34;

public class Service224 : IService38;

public class Service225 : IService17, IEnumerable<Service225>, IEnumerable
{
    public void Dispose() { }

    public IEnumerator<Service225> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service226 : IService1, IEnumerable<Service226>, IEnumerable
{
    public IEnumerator<Service226> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service227 : IService22
{
    public void Dispose() { }
}

internal class Service228 : IService15;

public class Service229 : IService5;

public class Service230 : IService30, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service231 : IService45;

public class Service232 : ServiceBase16, IService44
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service233 : IService42, IEquatable<Service233>, IEnumerable<Service233>, IEnumerable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service233? other)
    {
        return false;
    }

    public IEnumerator<Service233> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service234 : IService9
{
    public void Dispose() { }
}

public class Service235 : Service13, IService36, IEquatable<Service235>
{
    public bool Equals(Service235? other)
    {
        return false;
    }
}

public class Service236 : ServiceBase26, IService13;

public class Service237 : Service83, IService33, IDisposable
{
    public void Dispose() { }
}

public class Service238 : IService16, IDisposable
{
    public void Dispose() { }
}

public class Service239 : IService36;

public class Service240 : IService5;

public class Service241 : IService20
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service242 : IService1;

internal class Service243 : IService20, IDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service244 : IService36;

public class Service245 : IService44, IEquatable<Service245>
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service245? other)
    {
        return false;
    }
}

public class Service246 : IService15;

public class Service247 : IService1;

public class Service248 : ServiceBase13, IService35;

public class Service249 : IService26, IEquatable<Service249>, IEnumerable<Service249>, IEnumerable
{
    public bool Equals(Service249? other)
    {
        return false;
    }

    public IEnumerator<Service249> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service250 : IService41, IDisposable, IEnumerable<Service250>, IEnumerable
{
    public void Dispose() { }

    public IEnumerator<Service250> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service251 : IService46, IEnumerable<Service251>, IEnumerable
{
    public IEnumerator<Service251> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service252 : IService36, IDisposable
{
    public void Dispose() { }
}

internal class Service253 : IService20
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service254 : IService1, IDisposable
{
    public void Dispose() { }
}

public class Service255 : IService22
{
    public void Dispose() { }
}

public class Service256 : IService49;

internal class Service257 : IService17
{
    public void Dispose() { }
}

internal class Service258 : IService13;

internal class Service259 : ServiceBase5, IService49;

public class Service260 : IService33;

public class Service261 : IService36;

internal class Service262 : Service12, IService3, IEquatable<Service262>
{
    public bool Equals(Service262? other)
    {
        return false;
    }
}

public class Service263 : IService10, IDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service264 : IService34;

internal class Service265 : IService50;

internal class Service266 : ServiceBase33, IService11;

public class Service267 : IService19
{
    public void Dispose() { }
}

internal class Service268 : IService37;

public class Service269 : IService26;

public class Service270 : IService28, IDisposable, IEnumerable<Service270>, IEnumerable
{
    public void Dispose() { }

    public IEnumerator<Service270> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service271 : IService12;

public class Service272 : IService13;

internal class Service273 : IService42, IDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service274 : IService6;

public static class Service275;

public class Service276 : IService29, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service277 : IService41;

public class Service278 : IService31, IEnumerable<Service278>, IEnumerable
{
    public IEnumerator<Service278> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service279 : IService44, IDisposable, IEquatable<Service279>
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service279? other)
    {
        return false;
    }
}

internal class Service280 : IService43;

public class Service281 : IService34;

public class Service282 : IService40;

public class Service283 : IService37;

public class Service284 : IService48, IEnumerable<Service284>, IEnumerable
{
    public IEnumerator<Service284> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service285 : IService48;

public class Service286 : IService25, IEquatable<Service286>
{
    public bool Equals(Service286? other)
    {
        return false;
    }
}

public class Service287 : Service184, IService27;

internal class Service288 : IService25;

public class Service289 : Service52, IService32;

internal class Service290 : IService49;

public class Service291 : IService24;

public class Service292 : Service82, IService32, IEquatable<Service292>
{
    public bool Equals(Service292? other)
    {
        return false;
    }
}

internal class Service293 : IService25, IDisposable
{
    public void Dispose() { }
}

public class Service294 : IService19
{
    public void Dispose() { }
}

internal class Service295 : IService27;

internal class Service296 : IService40;

public class Service297 : IService7
{
    public void Dispose() { }
}

internal class Service298 : IService29;

public class Service299 : IService18;

public class Service300 : IService6, IEnumerable<Service300>, IEnumerable
{
    public IEnumerator<Service300> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service301 : IService37;

public class Service302 : IService3;

public class Service303 : IService16, IEnumerable<Service303>, IEnumerable
{
    public IEnumerator<Service303> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service304 : IService49, IEnumerable<Service304>, IEnumerable
{
    public IEnumerator<Service304> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service305 : ServiceBase33, IService40;

public class Service306 : IService2;

public class Service307 : IService21;

internal class Service308 : IService6;

public class Service309 : IService13;

internal class Service310 : IService3;

internal class Service311 : IService21;

public class Service312 : IService40;

public class Service313 : IService26, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service314 : IService1, IAsyncDisposable, IEquatable<Service314>
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service314? other)
    {
        return false;
    }
}

public class Service315 : IService4;

public class Service316 : Service300, IService23, IDisposable, IEnumerable<Service316>, IEnumerable
{
    public void Dispose() { }

    public new IEnumerator<Service316> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service317 : IService42, IEnumerable<Service317>, IEnumerable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public IEnumerator<Service317> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service318 : Service202, IService14;

public class Service319 : IService37;

public class Service320 : Service162, IService42
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service321 : IService11;

public class Service322 : IService5;

public class Service323 : ServiceBase40, IService27;

internal class Service324 : IService23;

public class Service325 : IService9
{
    public void Dispose() { }
}

public class Service326 : IService15;

internal class Service327 : IService36;

public class Service328 : Service97, IService18, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service329 : ServiceBase19, IService2
{
    public override void Dispose() { }
}

public class Service330 : IService12;

public class Service331 : ServiceBase17, IService36, IDisposable
{
    public override void Dispose() { }
}

public class Service332 : ServiceBase39, IService45;

public class Service333 : IService29, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service334 : IService40;

public class Service335 : IService34, IDisposable
{
    public void Dispose() { }
}

public class Service336 : IService35, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service337 : IService16;

internal class Service338 : IService41, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service339 : IService31;

public class Service340 : IService24;

public class Service341 : IService34;

public class Service342 : IService29;

internal class Service343 : IService4;

public class Service344 : IService11, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service345 : IService1;

public class Service346 : IService7, IEquatable<Service346>, IEnumerable<Service346>, IEnumerable
{
    public void Dispose() { }

    public bool Equals(Service346? other)
    {
        return false;
    }

    public IEnumerator<Service346> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service347 : IService34;

public class Service348 : IService30;

public class Service349 : IService3, IDisposable
{
    public void Dispose() { }
}

public class Service350 : ServiceBase8, IService32
{
    public override void Dispose() { }
}

public class Service351 : IService28;

internal class Service352 : IService13;

public class Service353 : IService26;

public class Service354 : IService27;

public class Service355 : Service164, IService33;

public class Service356 : IService48;

public class Service357 : IService16;

public class Service358 : ServiceBase15, IService3
{
    public override void Dispose() { }
}

public class Service359 : IService17
{
    public void Dispose() { }
}

public class Service360 : Service100, IService5;

public class Service361 : IService12, IEquatable<Service361>
{
    public bool Equals(Service361? other)
    {
        return false;
    }
}

public class Service362 : ServiceBase6, IService39;

public class Service363 : IService48;

internal class Service364 : IService35;

public class Service365 : IService41;

public class Service366 : IService49, IEnumerable<Service366>, IEnumerable
{
    public IEnumerator<Service366> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service367 : IService11;

public class Service368 : Service281, IService45, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service369 : IService14;

public class Service370 : IService26, IEnumerable<Service370>, IEnumerable
{
    public IEnumerator<Service370> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service371 : IService46;

public class Service372 : Service348, IService8
{
    public void Dispose() { }
}

public class Service373 : IService10
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service374 : IService22, IAsyncDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service375 : IService38;

public class Service376 : IService24;

public class Service377 : IService14;

public class Service378 : IService34, IEnumerable<Service378>, IEnumerable
{
    public IEnumerator<Service378> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service379 : IService11;

public class Service380 : ServiceBase44, IService10
{
    public override ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service381 : IService6;

public class Service382 : ServiceBase37, IService49;

internal class Service383 : IService18;

public class Service384 : IService10
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service385 : IService27;

public class Service386 : IService27;

internal class Service387 : IService11;

public class Service388 : IService31;

internal class Service389 : IService25;

public class Service390 : IService11;

internal class Service391 : IService39, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service392 : IService27, IEquatable<Service392>
{
    public bool Equals(Service392? other)
    {
        return false;
    }
}

public class Service393 : ServiceBase16, IService32, IDisposable
{
    public void Dispose() { }
}

public class Service394 : IService25, IEnumerable<Service394>, IEnumerable
{
    public IEnumerator<Service394> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service395 : IService24;

internal class Service396 : IService2, IDisposable
{
    public void Dispose() { }
}

public class Service397 : ServiceBase15, IService33
{
    public override void Dispose() { }
}

internal class Service398 : IService31;

public class Service399 : IService19, IEquatable<Service399>
{
    public void Dispose() { }

    public bool Equals(Service399? other)
    {
        return false;
    }
}

public class Service400 : IService8
{
    public void Dispose() { }
}

public class Service401 : IService14, IDisposable, IAsyncDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service402 : IService36, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service403 : IService17
{
    public void Dispose() { }
}

public class Service404 : IService30;

internal class Service405 : IService39;

public class Service406 : IService48;

internal class Service407 : IService7
{
    public void Dispose() { }
}

internal class Service408 : Service60, IService23;

public class Service409 : IService20
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service410 : IService6, IEnumerable<Service410>, IEnumerable
{
    public IEnumerator<Service410> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service411 : IService6, IDisposable
{
    public void Dispose() { }
}

public class Service412 : IService4;

public class Service413 : IService33, IEnumerable<Service413>, IEnumerable
{
    public IEnumerator<Service413> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service414 : IService43, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service415 : IService50;

public class Service416 : IService46;

internal class Service417 : IService43;

public class Service418 : IService18;

public class Service419 : IService41;

public class Service420 : IService5, IDisposable
{
    public void Dispose() { }
}

public class Service421 : IService47, IEquatable<Service421>, IEnumerable<Service421>, IEnumerable
{
    public bool Equals(Service421? other)
    {
        return false;
    }

    public IEnumerator<Service421> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service422 : IService33;

public class Service423 : IService9, IAsyncDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service424 : IService1;

internal class Service425 : Service29, IService21, IEquatable<Service425>
{
    public bool Equals(Service425? other)
    {
        return false;
    }
}

public class Service426 : Service270, IService7, IEquatable<Service426>
{
    public bool Equals(Service426? other)
    {
        return false;
    }
}

internal class Service427 : IService10, IDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service428 : IService45;

public class Service429 : IService47;

public class Service430 : ServiceBase13, IService16;

public class Service431 : IService26;

public class Service432 : IService38;

public class Service433 : Service19, IService9, IEnumerable<Service433>, IEnumerable
{
    public void Dispose() { }

    public IEnumerator<Service433> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service434 : IService35;

public class Service435 : Service50, IService40;

public class Service436 : IService8, IAsyncDisposable, IEquatable<Service436>
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service436? other)
    {
        return false;
    }
}

internal class Service437 : IService9
{
    public void Dispose() { }
}

public class Service438 : IService50;

public class Service439 : IService27;

public class Service440 : IService43;

public class Service441 : IService31;

public class Service442 : ServiceBase43, IService25;

public class Service443 : IService8
{
    public void Dispose() { }
}

public class Service444 : IService23;

internal class Service445 : IService24;

public class Service446 : Service84, IService48;

public class Service447 : IService17
{
    public void Dispose() { }
}

internal class Service448 : IService25;

public class Service449 : IService19
{
    public void Dispose() { }
}

public class Service450 : IService17
{
    public void Dispose() { }
}

internal class Service451 : IService23;

internal class Service452 : IService21;

public class Service453 : IService25;

public class Service454 : ServiceBase36, IService31, IAsyncDisposable, IEquatable<Service454>
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service454? other)
    {
        return false;
    }
}

public class Service455 : IService23, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service456 : IService23, IAsyncDisposable, IEquatable<Service456>
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service456? other)
    {
        return false;
    }
}

public class Service457 : ServiceBase12, IService18, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service458 : IService43, IDisposable
{
    public void Dispose() { }
}

internal class Service459 : ServiceBase4, IService39, IDisposable
{
    public void Dispose() { }
}

public class Service460 : IService49;

public class Service461 : ServiceBase40, IService13;

internal class Service462 : ServiceBase14, IService43, IEnumerable<Service462>, IEnumerable
{
    public IEnumerator<Service462> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service463 : IService24, IEnumerable<Service463>, IEnumerable
{
    public IEnumerator<Service463> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service464 : IService13;

public class Service465 : IService4;

internal class Service466 : IService19
{
    public void Dispose() { }
}

public class Service467 : IService12;

public class Service468 : IService48;

public class Service469 : IService46;

internal class Service470 : Service144, IService42, IDisposable
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service471 : IService26, IEquatable<Service471>
{
    public bool Equals(Service471? other)
    {
        return false;
    }
}

internal class Service472 : IService43, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service473 : IService24;

public class Service474 : ServiceBase42, IService36
{
    public override ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service475 : ServiceBase45, IService29, IDisposable
{
    public void Dispose() { }
}

internal class Service476 : Service155, IService30;

public class Service477 : IService7
{
    public void Dispose() { }
}

public class Service478 : IService34, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service479 : IService40;

public class Service480 : IService22, IEquatable<Service480>, IEnumerable<Service480>, IEnumerable
{
    public void Dispose() { }

    public bool Equals(Service480? other)
    {
        return false;
    }

    public IEnumerator<Service480> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Service481 : ServiceBase30, IService3, IEnumerable<Service481>, IEnumerable
{
    public IEnumerator<Service481> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service482 : IService18;

public class Service483 : IService43, IDisposable, IEnumerable<Service483>, IEnumerable
{
    public void Dispose() { }

    public IEnumerator<Service483> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service484 : IService26;

public class Service485 : Service334, IService18;

internal class Service486 : IService3, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service487 : ServiceBase6, IService44, IEquatable<Service487>
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public bool Equals(Service487? other)
    {
        return false;
    }
}

public class Service488 : IService43;

public class Service489 : IService17
{
    public void Dispose() { }
}

internal class Service490 : IService12, IEquatable<Service490>
{
    public bool Equals(Service490? other)
    {
        return false;
    }
}

public class Service491 : IService6;

internal class Service492 : IService49, IDisposable
{
    public void Dispose() { }
}

internal class Service493 : IService50;

internal class Service494 : Service271, IService46, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

internal class Service495 : IService9
{
    public void Dispose() { }
}

public class Service496 : IService10
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class Service497 : IService34, IEquatable<Service497>, IEnumerable<Service497>, IEnumerable
{
    public bool Equals(Service497? other)
    {
        return false;
    }

    public IEnumerator<Service497> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Service498 : IService41;

internal class Service499 : IService9, IEquatable<Service499>
{
    public void Dispose() { }

    public bool Equals(Service499? other)
    {
        return false;
    }
}

internal class Service500 : IService28, IDisposable
{
    public void Dispose() { }
}

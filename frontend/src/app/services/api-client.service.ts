import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AccountDto {
  id: string;
  name: string;
  currency: string;
  openingBalance: number;
  type: string;
  includeInZakat: boolean;
  isDebtAccount: boolean;
}

export interface TransactionDto {
  id: string;
  accountId: string;
  categoryId?: string;
  amount: number;
  currency: string;
  bookedOn: string;
  note?: string;
  tags: string[];
}

export interface ZakatHistoryDto {
  id: string;
  currency: string;
  amountDue: number;
  zakatableWealth: number;
  usedGoldStandard: boolean;
  hawlDays: number;
  createdUtc: string;
}

export interface CashflowPoint {
  date: string;
  inflow: number;
  outflow: number;
}

export interface CashflowReport {
  points: CashflowPoint[];
  generatedUtc: string;
}

export interface CategorySpendDto {
  category: string;
  amount: number;
  currency: string;
}

@Injectable({ providedIn: 'root' })
export class ApiClientService {
  private readonly baseUrl = environment.apiBaseUrl;
  readonly loading = signal(false);

  constructor(private readonly http: HttpClient) {}

  getAccounts(): Observable<AccountDto[]> {
    return this.http.get<AccountDto[]>(`${this.baseUrl}/accounts`);
  }

  getTransactions(params?: Record<string, string | number>): Observable<TransactionDto[]> {
    let query = new HttpParams();
    if (params) {
      for (const [key, value] of Object.entries(params)) {
        query = query.set(key, value.toString());
      }
    }

    return this.http.get<TransactionDto[]>(`${this.baseUrl}/transactions`, { params: query });
  }

  getDashboardSummary(): Observable<{ totalIncome: number; totalExpense: number; netCashFlow: number; generatedUtc: string; }> {
    return this.http.get<{ totalIncome: number; totalExpense: number; netCashFlow: number; generatedUtc: string; }>(`${this.baseUrl}/reports/summary`);
  }

  getZakatHistory(): Observable<ZakatHistoryDto[]> {
    return this.http.get<ZakatHistoryDto[]>(`${this.baseUrl}/zakat/history`);
  }

  calculateZakat(useGoldStandard: boolean): Observable<ZakatHistoryDto> {
    return this.http.post<ZakatHistoryDto>(`${this.baseUrl}/zakat/calculate`, { useGoldStandard });
  }

  getCashflowReport(params?: Record<string, string>): Observable<CashflowReport> {
    let query = new HttpParams();
    if (params) {
      for (const [key, value] of Object.entries(params)) {
        query = query.set(key, value);
      }
    }
    return this.http.get<CashflowReport>(`${this.baseUrl}/reports/cashflow`, { params: query });
  }

  getCategorySpend(params?: Record<string, string>): Observable<CategorySpendDto[]> {
    let query = new HttpParams();
    if (params) {
      for (const [key, value] of Object.entries(params)) {
        query = query.set(key, value);
      }
    }
    return this.http.get<CategorySpendDto[]>(`${this.baseUrl}/reports/category-spend`, { params: query });
  }
}

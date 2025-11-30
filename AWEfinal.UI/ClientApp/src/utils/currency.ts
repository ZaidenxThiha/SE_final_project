export function formatCurrency(value: number | undefined | null) {
  if (typeof value !== "number" || Number.isNaN(value)) {
    return "$0.00";
  }

  return `$${value.toLocaleString("en-US", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })}`;
}

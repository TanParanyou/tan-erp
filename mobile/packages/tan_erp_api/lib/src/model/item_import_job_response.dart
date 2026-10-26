//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/item_import_job_row_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_import_job_response.g.dart';

/// ItemImportJobResponse
///
/// Properties:
/// * [id] 
/// * [kind] 
/// * [mode] 
/// * [status] 
/// * [contentSha256] 
/// * [totalRows] 
/// * [processedRows] 
/// * [batchSize] 
/// * [totalBatches] 
/// * [completedBatches] 
/// * [rejectedBatches] 
/// * [createdCount] 
/// * [updatedCount] 
/// * [unchangedCount] 
/// * [errorRowCount] 
/// * [errorsTruncated] 
/// * [errors] 
/// * [failureCode] 
/// * [createdAtUtc] 
/// * [startedAtUtc] 
/// * [completedAtUtc] 
/// * [replayed] 
@BuiltValue()
abstract class ItemImportJobResponse implements Built<ItemImportJobResponse, ItemImportJobResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'mode')
  String? get mode;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'contentSha256')
  String? get contentSha256;

  @BuiltValueField(wireName: r'totalRows')
  int? get totalRows;

  @BuiltValueField(wireName: r'processedRows')
  int? get processedRows;

  @BuiltValueField(wireName: r'batchSize')
  int? get batchSize;

  @BuiltValueField(wireName: r'totalBatches')
  int? get totalBatches;

  @BuiltValueField(wireName: r'completedBatches')
  int? get completedBatches;

  @BuiltValueField(wireName: r'rejectedBatches')
  int? get rejectedBatches;

  @BuiltValueField(wireName: r'createdCount')
  int? get createdCount;

  @BuiltValueField(wireName: r'updatedCount')
  int? get updatedCount;

  @BuiltValueField(wireName: r'unchangedCount')
  int? get unchangedCount;

  @BuiltValueField(wireName: r'errorRowCount')
  int? get errorRowCount;

  @BuiltValueField(wireName: r'errorsTruncated')
  bool? get errorsTruncated;

  @BuiltValueField(wireName: r'errors')
  BuiltList<ItemImportJobRowResponse>? get errors;

  @BuiltValueField(wireName: r'failureCode')
  String? get failureCode;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'startedAtUtc')
  DateTime? get startedAtUtc;

  @BuiltValueField(wireName: r'completedAtUtc')
  DateTime? get completedAtUtc;

  @BuiltValueField(wireName: r'replayed')
  bool? get replayed;

  ItemImportJobResponse._();

  factory ItemImportJobResponse([void updates(ItemImportJobResponseBuilder b)]) = _$ItemImportJobResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemImportJobResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemImportJobResponse> get serializer => _$ItemImportJobResponseSerializer();
}

class _$ItemImportJobResponseSerializer implements PrimitiveSerializer<ItemImportJobResponse> {
  @override
  final Iterable<Type> types = const [ItemImportJobResponse, _$ItemImportJobResponse];

  @override
  final String wireName = r'ItemImportJobResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemImportJobResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.mode != null) {
      yield r'mode';
      yield serializers.serialize(
        object.mode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.contentSha256 != null) {
      yield r'contentSha256';
      yield serializers.serialize(
        object.contentSha256,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.totalRows != null) {
      yield r'totalRows';
      yield serializers.serialize(
        object.totalRows,
        specifiedType: const FullType(int),
      );
    }
    if (object.processedRows != null) {
      yield r'processedRows';
      yield serializers.serialize(
        object.processedRows,
        specifiedType: const FullType(int),
      );
    }
    if (object.batchSize != null) {
      yield r'batchSize';
      yield serializers.serialize(
        object.batchSize,
        specifiedType: const FullType(int),
      );
    }
    if (object.totalBatches != null) {
      yield r'totalBatches';
      yield serializers.serialize(
        object.totalBatches,
        specifiedType: const FullType(int),
      );
    }
    if (object.completedBatches != null) {
      yield r'completedBatches';
      yield serializers.serialize(
        object.completedBatches,
        specifiedType: const FullType(int),
      );
    }
    if (object.rejectedBatches != null) {
      yield r'rejectedBatches';
      yield serializers.serialize(
        object.rejectedBatches,
        specifiedType: const FullType(int),
      );
    }
    if (object.createdCount != null) {
      yield r'createdCount';
      yield serializers.serialize(
        object.createdCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.updatedCount != null) {
      yield r'updatedCount';
      yield serializers.serialize(
        object.updatedCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.unchangedCount != null) {
      yield r'unchangedCount';
      yield serializers.serialize(
        object.unchangedCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.errorRowCount != null) {
      yield r'errorRowCount';
      yield serializers.serialize(
        object.errorRowCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.errorsTruncated != null) {
      yield r'errorsTruncated';
      yield serializers.serialize(
        object.errorsTruncated,
        specifiedType: const FullType(bool),
      );
    }
    if (object.errors != null) {
      yield r'errors';
      yield serializers.serialize(
        object.errors,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ItemImportJobRowResponse)]),
      );
    }
    if (object.failureCode != null) {
      yield r'failureCode';
      yield serializers.serialize(
        object.failureCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.startedAtUtc != null) {
      yield r'startedAtUtc';
      yield serializers.serialize(
        object.startedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.completedAtUtc != null) {
      yield r'completedAtUtc';
      yield serializers.serialize(
        object.completedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.replayed != null) {
      yield r'replayed';
      yield serializers.serialize(
        object.replayed,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ItemImportJobResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemImportJobResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'mode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.mode = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'contentSha256':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.contentSha256 = valueDes;
          break;
        case r'totalRows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.totalRows = valueDes;
          break;
        case r'processedRows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.processedRows = valueDes;
          break;
        case r'batchSize':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.batchSize = valueDes;
          break;
        case r'totalBatches':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.totalBatches = valueDes;
          break;
        case r'completedBatches':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.completedBatches = valueDes;
          break;
        case r'rejectedBatches':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.rejectedBatches = valueDes;
          break;
        case r'createdCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.createdCount = valueDes;
          break;
        case r'updatedCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.updatedCount = valueDes;
          break;
        case r'unchangedCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.unchangedCount = valueDes;
          break;
        case r'errorRowCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.errorRowCount = valueDes;
          break;
        case r'errorsTruncated':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.errorsTruncated = valueDes;
          break;
        case r'errors':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ItemImportJobRowResponse)]),
          ) as BuiltList<ItemImportJobRowResponse>?;
          if (valueDes == null) continue;
          result.errors.replace(valueDes);
          break;
        case r'failureCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.failureCode = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'startedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.startedAtUtc = valueDes;
          break;
        case r'completedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.completedAtUtc = valueDes;
          break;
        case r'replayed':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.replayed = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ItemImportJobResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemImportJobResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}



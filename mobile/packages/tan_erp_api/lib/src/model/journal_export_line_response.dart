//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'journal_export_line_response.g.dart';

/// JournalExportLineResponse
///
/// Properties:
/// * [batchRef] 
/// * [messageId] 
/// * [dedupeKey] 
/// * [kind] 
/// * [resourceNumber] 
/// * [debitAccountCode] 
/// * [debitAccountName] 
/// * [creditAccountCode] 
/// * [creditAccountName] 
/// * [amount] 
/// * [currency] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class JournalExportLineResponse implements Built<JournalExportLineResponse, JournalExportLineResponseBuilder> {
  @BuiltValueField(wireName: r'batchRef')
  String? get batchRef;

  @BuiltValueField(wireName: r'messageId')
  String? get messageId;

  @BuiltValueField(wireName: r'dedupeKey')
  String? get dedupeKey;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'resourceNumber')
  String? get resourceNumber;

  @BuiltValueField(wireName: r'debitAccountCode')
  String? get debitAccountCode;

  @BuiltValueField(wireName: r'debitAccountName')
  String? get debitAccountName;

  @BuiltValueField(wireName: r'creditAccountCode')
  String? get creditAccountCode;

  @BuiltValueField(wireName: r'creditAccountName')
  String? get creditAccountName;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  JournalExportLineResponse._();

  factory JournalExportLineResponse([void updates(JournalExportLineResponseBuilder b)]) = _$JournalExportLineResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(JournalExportLineResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<JournalExportLineResponse> get serializer => _$JournalExportLineResponseSerializer();
}

class _$JournalExportLineResponseSerializer implements PrimitiveSerializer<JournalExportLineResponse> {
  @override
  final Iterable<Type> types = const [JournalExportLineResponse, _$JournalExportLineResponse];

  @override
  final String wireName = r'JournalExportLineResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    JournalExportLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.batchRef != null) {
      yield r'batchRef';
      yield serializers.serialize(
        object.batchRef,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.messageId != null) {
      yield r'messageId';
      yield serializers.serialize(
        object.messageId,
        specifiedType: const FullType(String),
      );
    }
    if (object.dedupeKey != null) {
      yield r'dedupeKey';
      yield serializers.serialize(
        object.dedupeKey,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.resourceNumber != null) {
      yield r'resourceNumber';
      yield serializers.serialize(
        object.resourceNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.debitAccountCode != null) {
      yield r'debitAccountCode';
      yield serializers.serialize(
        object.debitAccountCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.debitAccountName != null) {
      yield r'debitAccountName';
      yield serializers.serialize(
        object.debitAccountName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.creditAccountCode != null) {
      yield r'creditAccountCode';
      yield serializers.serialize(
        object.creditAccountCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.creditAccountName != null) {
      yield r'creditAccountName';
      yield serializers.serialize(
        object.creditAccountName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
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
  }

  @override
  Object serialize(
    Serializers serializers,
    JournalExportLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required JournalExportLineResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'batchRef':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.batchRef = valueDes;
          break;
        case r'messageId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.messageId = valueDes;
          break;
        case r'dedupeKey':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.dedupeKey = valueDes;
          break;
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'resourceNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.resourceNumber = valueDes;
          break;
        case r'debitAccountCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.debitAccountCode = valueDes;
          break;
        case r'debitAccountName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.debitAccountName = valueDes;
          break;
        case r'creditAccountCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.creditAccountCode = valueDes;
          break;
        case r'creditAccountName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.creditAccountName = valueDes;
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  JournalExportLineResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = JournalExportLineResponseBuilder();
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


